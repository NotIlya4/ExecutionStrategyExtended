﻿namespace EntityFrameworkCore.ExecutionStrategyExtended;

public interface IResponseSerializer
{
    string Serialize<T>(T obj);
    T Deserialize<T>(string rawObj);
}