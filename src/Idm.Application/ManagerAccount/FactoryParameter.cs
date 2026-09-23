using Idm.Domain.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;

namespace Idm.Application.ManagerAccount;

public abstract class Parameter
{
    // Метод теперь абстрактный, возвращает NpgsqlParameter
    public abstract NpgsqlParameter CreateParameter(NpgsqlCommand command, string nameParameter);
}

public class IntParameter : Parameter
{
    public int Value { get; set; }
    
    public override NpgsqlParameter CreateParameter(NpgsqlCommand command, string nameParameter)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = nameParameter;
        parameter.Value = Value;
        parameter.NpgsqlDbType = NpgsqlDbType.Integer;
        return parameter;
    }
}

public class StringParameter : Parameter
{
    public string Value { get; set; } = string.Empty;
    
    public override NpgsqlParameter CreateParameter(NpgsqlCommand command, string nameParameter)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = nameParameter;
        parameter.Value = Value;
        parameter.NpgsqlDbType = NpgsqlDbType.Varchar;
        return parameter;
    }
}

public class DateTimeParameter : Parameter
{
    public DateTime Value { get; set; } = DateTime.UtcNow;
    
    public override NpgsqlParameter CreateParameter(NpgsqlCommand command, string nameParameter)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = nameParameter;
        parameter.Value = Value;
        parameter.NpgsqlDbType = NpgsqlDbType.TimestampTz;
        return parameter;
    }
}

public class BoolParameter : Parameter
{
    public bool Value { get; set; }
    
    public override NpgsqlParameter CreateParameter(NpgsqlCommand command, string nameParameter)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = nameParameter;
        parameter.Value = Value;
        parameter.NpgsqlDbType = NpgsqlDbType.Boolean;
        return parameter;
    }
}

public static class FactoryParameter
{
    // Передаем object value в метод фабрики вместо статических свойств класса
    public static void CreateAndAddParameter(NpgsqlCommand command, string nameParameter, string type, object value)
    {
        Parameter parameter = type.ToLower() switch
        {
            "integer"  => new IntParameter { Value = Convert.ToInt32(value) },
            "varchar"  => new StringParameter { Value = value?.ToString() ?? string.Empty },
            "datetime" => new DateTimeParameter { Value = Convert.ToDateTime(value) },
            "bool"     => new BoolParameter { Value = Convert.ToBoolean(value) },
            _          => throw new ArgumentException($"Неизвестный тип параметра: {type}")
        };
        
        var npgsqlParam = parameter.CreateParameter(command, nameParameter);
        command.Parameters.Add(npgsqlParam);
    }
}
