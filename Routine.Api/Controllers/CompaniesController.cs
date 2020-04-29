﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Routine.Api.ActionContraints;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route(template: "api/companies")]
    public class CompaniesController :ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CompaniesController(ICompanyRepository companyRepository, 
            IMapper mapper, 
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ?? 
                                throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));

            _propertyCheckerService = propertyCheckerService?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet(Name = nameof(GetCompanies))]
        //support HEAD
        [HttpHead]
        //public async Task<IActionResult> GetCompanies()
        public async Task<IActionResult> GetCompanies(
            [FromQuery]CompanyDtoParameters parameters)
        {
            //解决搜索的时候column名字不存在会返回500服务器错的问题，应该是客户发送错误。
            if (!_propertyMappingService.ValidMappingExistsFor<CompanyDto, Company>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }

            var companies = await _companyRepository.GetCompaniesAsync(parameters);
            //var companyDtos = new List<CompanyDto>();
            //foreach(var company in companies)
            //{
            //    companyDtos.Add(new CompanyDto
            //    {
            //        Id = company.Id,
            //        Name = company.Name
            //    });
            //}

            //move to "links"
            //var previousPageLink = companies.HasPrevious
            //                    ?
            //                    CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage)
            //                    : null;

            //var nextPageLink = companies.HasNext
            //                    ?
            //                    CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage)
            //                    : null;

            var paginationMetadata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages
                //previousPageLink,
                //nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata ,new JsonSerializerOptions
            {
                //to fix issue "&" conver to "/uXXX" in the link
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            var shapedData = companyDtos.ShapeData(parameters.Fields);

            var links = CreateLinksForCompany(parameters, companies.HasPrevious, companies.HasNext);

            var shapedCompaniesWithLinks = shapedData.Select(c =>
            {
                var companyDict = c as IDictionary<string, object>;
                var companyLinks = CreateLinksForCompany((Guid)companyDict["Id"], null);
                companyDict.Add("links", companyLinks);
                return companyDict;
            });

            var linkedCollectionResource = new
            {
                value = shapedCompaniesWithLinks,
                links
            };
            return Ok(linkedCollectionResource);
        }


        //produce是过滤器，针对下面的“Accept”
        [Produces("application/json",
            "application/vnd.company.hateoas+json",
            "application/vnd.company.company.friendly+json",
            "application/vnd.company.company.friendly.hateoas+json",
            "application/vnd.company.company.full+json",
            "application/vnd.company.company.full.hateoas+json")]
        [HttpGet(template:"{companyId}", Name = nameof(GetCompany))]   //api/companies/{companyId}
        public async Task<IActionResult> GetCompany(Guid companyId, string fields,
            //为了处理Vender-specific media types从header内把accept取出来
            [FromHeader(Name ="Accept")] string mediaType)
        {
            if(!MediaTypeHeaderValue.TryParse(mediaType,out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }

            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> myLinks = new List<LinkDto>();
            if (includeLinks)
            {
                myLinks = CreateLinksForCompany(companyId, fields);
            }

            var primaryMediaType = includeLinks ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) :
                parsedMediaType.SubTypeWithoutSuffix;

            if(primaryMediaType == "vnd.company.company.full")
            {
                var full = _mapper.Map<CompanyFullDto>(company).ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    full.Add("links", myLinks);
                }

                return Ok(full);
            }

            var friendly = _mapper.Map<CompanyDto>(company).ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendly.Add("links", myLinks);
            }

            return Ok(friendly);

            //if (parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
            //{

            //    var links = CreateLinksForCompany(companyId, fields);

            //    var linkedDict = _mapper.Map<CompanyDto>(company).ShapeData(fields) as IDictionary<string, object>;

            //    linkedDict.Add("links", links);
            //    return Ok(linkedDict);
            //}

            //return Ok(_mapper.Map<CompanyDto>(company).ShapeData(fields));

        }

        [HttpPost(Name = nameof(CreateCompanyWithBankruptTime))]
        //apicontroller will convert parameters from body to here
        [RequestHeaderMatchesMediaType("Content-Type", "application/vnd.company.companyforcreationwithbankrupttime+json")]
        [Consumes("application/vnd.company.companyforcreationwithbankrupttime+json")]

        public async Task<ActionResult<CompanyDto>> CreateCompanyWithBankruptTime([FromBody]CompanyAddWithBankruptTimeDto company)
        {
            if (company == null)
                return BadRequest();

            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);
            var links = CreateLinksForCompany(returnDto.Id, null);

            var linkedDict = returnDto.ShapeData(null) as IDictionary<string, object>;

            linkedDict.Add("links", links);
            return CreatedAtRoute(nameof(GetCompanies), new { companyId = linkedDict["Id"] }, linkedDict);
        }

        [HttpPost(Name =nameof(CreateCompany))]
        [RequestHeaderMatchesMediaType("Content-Type", "application/json", 
            "application/vnd.company.companyforcreation+json")]
        [Consumes("application/json", "application/vnd.company.companyforcreation+json")]
        //apicontroller will convert parameters from body to here
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody]CompanyAddDto company)
        {
            if (company == null)
                return BadRequest();

            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);
            var links = CreateLinksForCompany(returnDto.Id, null);

            var linkedDict = returnDto.ShapeData(null) as IDictionary<string, object>;

            linkedDict.Add("links", links);
            return CreatedAtRoute(nameof(GetCompanies), new { companyId = linkedDict["Id"] }, linkedDict);
        }



        [HttpDelete("{companyId}", Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository .GetCompanyAsync(companyId);

            if (companyEntity==null)
            {
                return NotFound();
            }

            //purpose: load all employees
            await _companyRepository.GetEmployeesAsync(companyId, null);
            _companyRepository.DeleteCompany(companyEntity);

            await _companyRepository.SaveAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return Ok();
        }

        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                //case ResourceUriType.CurrentPage:
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link(nameof(GetCompany),
                    new { companyId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link(nameof(GetCompany),
                    new { companyId, fields }),
                    "self",
                    "GET"));
            }


            links.Add(
                new LinkDto(Url.Link(nameof(DeleteCompany),
                new { companyId }),
                "delete_company",
                "DELETE"));

            links.Add(
                new LinkDto(Url.Link(nameof(EmployeesController.CreateEmployeeForCompany),
                new { companyId }),
                "create_employee_fo_company",
                "POST"));


            links.Add(
                new LinkDto(Url.Link(nameof(EmployeesController.GetEmployeesForCompany),
                new { companyId }),
                "employees",
                "GET"));
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.CurrentPage), "self", "GET"));
            
            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage), "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage), "next_page", "GET"));
            }
            return links;
        }
    }
}
