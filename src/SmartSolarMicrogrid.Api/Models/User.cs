/*
 * Purpose: Backoffice or GridOperator user.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SmartSolarMicrogrid.Api.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Backoffice" | "GridOperator"
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
