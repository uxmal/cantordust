using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GhidraScript
{
    public string sourceFile;
    public string currentProgram;

    public GhidraProgram getCurrentProgram()
    {
        throw new NotImplementedException();
    }
}

class GhidraProgram
{
    public Address getMinAddress() { throw new NotImplementedException(); }
}

class Address
{
    internal string ToString(bool v)
    {
        throw new NotImplementedException();
    }
}