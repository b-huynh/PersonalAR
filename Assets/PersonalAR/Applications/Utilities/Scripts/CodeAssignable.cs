using System;

public class CodeAssignable
{
    public Guid AssignableID { get; private set; }

    public CodeAssignable(string guid)
    {
        this.AssignableID = new Guid(guid);
    }

    public CodeAssignable()
    {
        this.AssignableID = Guid.NewGuid();
    }

    public bool IsSameAssignable(CodeAssignable otherAssignable)
    {
        return this.AssignableID == otherAssignable.AssignableID;
    }
}
