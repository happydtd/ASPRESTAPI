using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Models;
using Routine.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route(template: "api/companies/{companyId}/employees")]
    //[ResponseCache(CacheProfileName = "120sCacheProfile")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class EmployeesController : ControllerBase
    {
        private ICompanyRepository _companyRepository;
        private IMapper _mapper;
        public EmployeesController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository ??
                    throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }


        [HttpGet(Name = nameof(GetEmployeesForCompany))]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesForCompany(Guid companyId, 
            [FromQuery] EmployeeDtoParameters parameters)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employees = await _companyRepository
                .GetEmployeesAsync(companyId, parameters);
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeeDtos);
        }

        [HttpGet("{employeeId}", Name = nameof(GetEmployeeForCompany))]
        //只是标示，没缓存器，需要在configureservices中设置
        //[ResponseCache(Duration = 60)]
        [HttpCacheExpiration(CacheLocation= CacheLocation.Public, MaxAge =1800)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employee = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return Ok(employeeDto);
        }

        [HttpPost(Name = nameof(CreateEmployeeForCompany))]
        public async Task<ActionResult<CompanyDto>> CreateEmployeeForCompany(Guid companyId, EmployeeAddDto employee)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var entity = _mapper.Map<Employee>(employee);
            _companyRepository.AddEmployee(companyId, entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<EmployeeDto>(entity);
            return CreatedAtRoute(nameof(GetEmployeeForCompany), new { companyId = companyId, employeeId = returnDto.Id }, returnDto);

        }

        [HttpPut("{employeeId}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeUpdateDto employee)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            if (employeeEntity == null)
            {
                var employeeToAddEntity = _mapper.Map<Employee>(employee);
                employeeToAddEntity.Id = employeeId;

                _companyRepository.AddEmployee(companyId, employeeToAddEntity);
                await _companyRepository.SaveAsync();

                var returnDto = _mapper.Map<EmployeeDto>(employeeEntity);
                return CreatedAtRoute(nameof(GetEmployeeForCompany), new { companyId = companyId, employeeId = returnDto.Id }, returnDto);

            }

            //convert entity to updateDto
            //update employee values to updateDto
            //send updateDto back to entity
            _mapper.Map(employee, employeeEntity);
            _companyRepository.UpdateEmployee(employeeEntity);
            await _companyRepository.SaveAsync();

            //return 204, return code depend client request.
            return NoContent();
        }

        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid employeeId, 
            JsonPatchDocument<EmployeeUpdateDto> patchDocument)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            if (employeeEntity == null)
            {
                var employeeDto = new EmployeeUpdateDto();
                patchDocument.ApplyTo(employeeDto, ModelState);

                if (!TryValidateModel(employeeDto))
                {
                    return ValidationProblem(ModelState);
                }

                var employeeToAdd = _mapper.Map<Employee>(employeeDto);
                employeeToAdd.Id = employeeId;

                _companyRepository.AddEmployee(companyId, employeeToAdd);

                await _companyRepository.SaveAsync();

                var dtoToReturn = _mapper.Map<EmployeeDto>(employeeToAdd);

                return CreatedAtRoute(nameof(GetEmployeeForCompany), new
                {
                    companyId,
                    employeeId = dtoToReturn.Id
                }, dtoToReturn);
            }

            var dtoToPatch = _mapper.Map<EmployeeUpdateDto>(employeeEntity);
            //需要处理验证错误, 如果remove不存在的field，应该返回400错误
            patchDocument.ApplyTo(dtoToPatch,ModelState);
            // 不然执行remove required field时会返回500错误，应该是400错误。

            if (!TryValidateModel(dtoToPatch))
            {
                //下面有override方法
                return ValidationProblem(ModelState);
            }

            _mapper.Map(dtoToPatch, employeeEntity);
            _companyRepository.UpdateEmployee(employeeEntity);
            await _companyRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            if (!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);

            if (employeeEntity == null)
            {
                return NotFound();
            }

            _companyRepository.DeleteEmployee(employeeEntity);

            await _companyRepository.SaveAsync();

            return NoContent();
        }

        //自定义ValidationProblem(ModelState)返回code
        public override ActionResult ValidationProblem(ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
