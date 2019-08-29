//using System;
//using System.Collections.Generic;
//using System.Linq;
//using YamlDotNet.Serialization;
//using YamlDotNet.Serialization.TypeInspectors;

//namespace SwaggerWithVersionAndYaml
//{
//    public class PropertiesIgnoreTypeInspector : TypeInspectorSkeleton
//    {
//        private readonly ITypeInspector typeInspector;
//        public PropertiesIgnoreTypeInspector(ITypeInspector typeInspector)
//        {
//            this.typeInspector = typeInspector;
//        }
//        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
//        {
//            return typeInspector.GetProperties(type, container).Where(p => p.Name != "extensions" && p.Name != "operation-id");
//        }
//    }
//}
