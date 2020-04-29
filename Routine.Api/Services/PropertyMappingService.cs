using Routine.Api.Entities;
using Routine.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Routine.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _companyPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyName", new PropertyMappingValue(new List<string>{"Name"}) },
                {"Country", new PropertyMappingValue(new List<string>{"Country"}) },
                {"Industry", new PropertyMappingValue(new List<string>{ "Industry" }) },
                {"Product", new PropertyMappingValue(new List<string>{"Product"}) },
                {"Introduction", new PropertyMappingValue(new List<string>{"Introduction"}) }

            };

        private readonly Dictionary<string, PropertyMappingValue> _employeePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyId", new PropertyMappingValue(new List<string>{"CompanyId"}) },
                {"EmployeeNo", new PropertyMappingValue(new List<string>{"EmployeeNo"}) },
                {"Name", new PropertyMappingValue(new List<string>{"FirstName", "LastName"}) },
                {"GenderDisplay", new PropertyMappingValue(new List<string>{"Gender"}) },
                {"Age", new PropertyMappingValue(new List<string>{"DateOfBirth"}, true) }

            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<EmployeeDto, Employee>(_employeePropertyMapping));
            _propertyMappings.Add(new PropertyMapping<CompanyDto, Company>(_companyPropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            var propertyMappings = matchingMapping.ToList();
            if (propertyMappings.Count() == 1)
            {
                return propertyMappings.First().MappingDictionary;
            }

            throw new Exception($"can not find relationship of  {typeof(TSource)} and {typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldAfterSplit = fields.Split(",");
            foreach(var field in fieldAfterSplit)
            {
                var trimmedFields = field.Trim();
                var indexOfFirstSpace = trimmedFields.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedFields : trimmedFields.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
