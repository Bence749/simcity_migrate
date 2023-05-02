using System;

namespace SimCity.Persistence;

public class PersistenceExceptions : Exception
{
    public PersistenceExceptions() {}
    
    public PersistenceExceptions(string msg) : base(msg) {}
    
    public PersistenceExceptions(string msg, Exception innerException) : base(msg, innerException) {}
}