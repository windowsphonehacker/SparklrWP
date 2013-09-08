
using System;
namespace SparklrLib.Objects.Responses.Work
{
    public class Username : IEquatable<Username>
    {
        public string username { get; set; }
        public string displayname { get; set; }
        public int id { get; set; }

        public override string ToString()
        {
            return id + ": " + displayname + " (@" + username + ")";
        }

        public override int GetHashCode()
        {
            return id;
        } 

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Username u = obj as Username;
            if ((object)u == null)
            {
                return false;
            }

            // Return true if the fields match:
            return id == u.id;
        }

        public bool Equals(Username u)
        {
            // If parameter is null return false:
            if ((object)u == null)
            {
                return false;
            }

            // Return true if the fields match:
            return id == u.id;
        }

        public bool Equals(int newid)
        {
            // If parameter is null return false:
            if ((object)newid == null)
            {
                return false;
            }

            // Return true if the fields match:
            return id == newid;
        }

    }
}