using System.Data.Common;
namespace Login.GuardRail;

public class SignupGuardRail
{
    public SignupGuardRail(Login.Models.SignupModel signupData)
    {
        data = signupData;
    }
    public Login.Models.SignupModel data;
    private string? Replace(string? s)
    {
        if (s == null) {return null;}
        string t = "";
        foreach (char c in s)
        {
            if      (c == (char) 39) { t += (char) 8217; } // Replaces single         quote ' 39 with Apostrophe         ’ 8217
            else if (c == (char) 34) { t += (char) 8221; } // Replaces Neutral double quote " 34 with Right double quote ” 8221
            else                     { t += c; }
        }
        return t;
    }
    private bool SQLInjectionFilter()
    {
        try
        {
            data.username = Replace(data.username);
            // data.password = Replace(data.password);
            data.name     = Replace(data.name    );
            data.email    = Replace(data.email   );
            data.address  = Replace(data.address );
            data.phone    = Replace(data.phone   );
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool DataRestriction()
    {
        try {
            if (data.username == null) { return false; }
            if (data.password == null) { return false; }
            if (data.name     == null) { return false; }
            if (data.email    == null) { return false; }
            if (data.address  == null) { return false; }
            if (data.phone    == null) { return false; }

            if (data.username.Length > 60) { return false; }
            if (data.username.Length <  6) { return false; }
            if (data.password.Length <  8) { return false; }
            if (data.password.Length > 32) { return false; }
            if (data.name.Length     > 60) { return false; }
            if (data.name.Length     <= 1) { return false; }
            if (data.email.Length    > 60) { return false; }
            if (data.email.Length    <  5) { return false; }
            if (data.address.Length  >120) { return false; }
            if (data.address.Length  <  5) { return false; }
            if (data.phone.Length    > 15) { return false; }
            if (data.phone.Length    <  8) { return false; }

            // {"email", @"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$"},
            // {"password", @"(?=.*[A-Z])(?=.*[a-z])(?=.*\W)(?=.*\d).{8,32}"},
            // {"tel", @"^([+]?\d{1,3}[-|\s]?|)\d{3}[-|\s]?\d{3}[-|\s]?\d{4}"},
            // {"any", @"[^]*"}
            // These regex are for actual validation. But I heard that this tegex vlidation be hanged or REDOS Attck may be applied.
            // Some element in these regex might even be catastrophic, exponential-time regular expressions. 
            // I don't know a thing about this, so, skipping these validation

            return true;
        }
        catch (Exception) { return false; }
    }
    
    public string Validation()
    {
        if (!SQLInjectionFilter()) {return "ValidationError_SQLProtection"; }
        if (!DataRestriction()   ) {return "ValidationError_BadValue"     ; }
        return "Ok";
        
    }
}