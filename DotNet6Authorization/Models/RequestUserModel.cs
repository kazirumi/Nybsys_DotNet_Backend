using System;
using System.Collections.Generic;

namespace DotNet6Authorization.Models;

public partial class RequestUserModel
{

    public string? UserName { get; set; }

    public string? Email { get; set; }
    public string? FullName { get; set; }


}
