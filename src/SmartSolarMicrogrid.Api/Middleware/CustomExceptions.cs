/*
 * Purpose: Custom Exceptions for global handling.
 * Author: Rathnayaka D.M.B.H. (IT22149930)
 * Date: 2026-09-22
 */
using System;

namespace SmartSolarMicrogrid.Api.Middleware
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
    
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
