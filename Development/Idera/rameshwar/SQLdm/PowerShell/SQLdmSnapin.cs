/// <summary>
/// Create this sample as an PowerShell snap-in
/// </summary>
using System.ComponentModel;
using System.Management.Automation;

[RunInstaller(true)]
public class SQLdmSnapin : PSSnapIn
{
    /// <summary>
    /// Create an instance of the SQLdmSnapin
    /// </summary>
    public SQLdmSnapin()
        : base()
    {
    }

    /// <summary>
    /// Get a name for this PowerShell snap-in. This name will be used in registering
    /// this PowerShell snap-in.
    /// </summary>
    public override string Name
    {
        get
        {
            return "SQLdmSnapin";
        }
    }

    /// <summary>
    /// Vendor information for this PowerShell snap-in.
    /// </summary>
    public override string Vendor
    {
        get
        {
            return "Idera";
        }
    }

    /// <summary>
    /// Gets resource information for vendor. This is a string of format: 
    /// resourceBaseName,resourceName. 
    /// </summary>
    public override string VendorResource
    {
        get
        {
            return "SQLdmSnapin,Idera";
        }
    }

    /// <summary>
    /// Description of this PowerShell snap-in.
    /// </summary>
    public override string Description
    {
        get
        {
            return "This is a PowerShell snap-in for automating certain SQLDM actions.";
        }
    }

    public override string[] Formats
    {
        get
        {
            return new string[] { "SQLdmSnapin-Formats.ps1xml" }; 
        }
    }

}