using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit
{
    public static class MKTypeExtensions
    {


        public static bool IsStatic(this Type _type)
        {
            return _type.IsAbstract && _type.IsSealed;
        }
    }
} // Minikit namespace
