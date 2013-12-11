using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBlocks.LiveEditing.Helper
{
    // C - Create A - Update D - Delete M - Move O - Copy S - Sort K - Rollback 
    // P - Public Access I - Manage Hostname U - Publish R - Permissions 
    // Z - Audit Trail : - Edit in Canvas 5 - Send to translation F - Browse Name 
    // 4 - Translate H - Send To Publish
    
    /// <summary>
    /// Helper to check current user permissions
    /// </summary>
    public class UserPermissions
    {
        public static bool AllowedToPublish(string permissions)
        {
            return permissions.Contains('U');
        }

        public static bool AllowedToUpdate(string permissions)
        {
            return permissions.Contains('A');
        }

        public static bool AllowedToSendToPublish(string permissions)
        {
            return permissions.Contains('H');
        }
    }
}