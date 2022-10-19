using System;

namespace WpfUI.Exceptions;

public class ResourceBrushNotFoundException : Exception
{
    public ResourceBrushNotFoundException(string message) : base(message)
    {
    }

    public ResourceBrushNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}