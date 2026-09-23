using Idm.Domain.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;

namespace Idm.Application.ManagerAccount;

public abstract class Parameter
{
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
public class LongParameter : Parameter
{
    public long Value { get; set; }

    public override NpgsqlParameter CreateParameter(NpgsqlCommand command, string nameParameter)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = nameParameter;
        parameter.Value = Value;
        parameter.NpgsqlDbType = NpgsqlDbType.Bigint;
        return parameter;
    }
}

public class VarcharParameter : Parameter
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
    public static void CreateAndAddParameter(NpgsqlCommand command, string nameParameter, FactoryType type, object value)
    {
        Parameter parameter = type switch
        {
            FactoryType.Integer => new IntParameter { Value = Convert.ToInt32(value) },
            FactoryType.Long => new LongParameter { Value = Convert.ToInt64(value) },
            FactoryType.Varchar => new VarcharParameter { Value = value?.ToString() ?? string.Empty },
            FactoryType.DateTime => new DateTimeParameter { Value = Convert.ToDateTime(value) },
            FactoryType.Boolean => new BoolParameter { Value = Convert.ToBoolean(value) },
            _ => throw new ArgumentException($"Неизвестный тип параметра: {type}")
        };

        var npgsqlParam = parameter.CreateParameter(command, nameParameter);
        command.Parameters.Add(npgsqlParam);
    }
}

public enum FactoryType
{
    Integer,
    Varchar,
    DateTime,
    Boolean,
    Long

}
