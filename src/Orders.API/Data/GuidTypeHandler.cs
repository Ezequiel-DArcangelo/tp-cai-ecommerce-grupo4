using Dapper;
using System.Data;

namespace Orders.API.Data
{
    // Le dice a Dapper como convertir entre Guid (C#) y string (SQLite)
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            // Al guardar: convertir Guid a string
            parameter.Value = value.ToString();
        }

        public override Guid Parse(object value)
        {
            // Al leer: convertir string a Guid
            return Guid.Parse(value.ToString());
        }
    }
}