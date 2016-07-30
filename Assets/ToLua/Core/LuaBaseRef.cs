﻿/*
Copyright (c) 2015-2016 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using UnityEngine;

namespace LuaInterface
{
    public abstract class LuaBaseRef : IDisposable
    {
        public string name = null;
        protected int reference = -1;
        protected LuaState luaState;
        protected ObjectTranslator translator = null;

        protected bool beDisposed;
        protected int count = 0;

        public LuaBaseRef()
        {
            count = 1;
        }

        ~LuaBaseRef()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            --count;

            if (count > 0)
            {
                return;
            }

            Dispose(true);            
        }

        public void AddRef()
        {
            ++count;            
        }

        public virtual void Dispose(bool disposeManagedResources)
        {
            if (!beDisposed)
            {
                beDisposed = true;   

                if (reference > 0 && luaState != null)
                {
                    luaState.CollectRef(reference, name, !disposeManagedResources);
                }
                
                reference = -1;
                luaState = null;
                count = 0;
            }            
        }

        public LuaState GetLuaState()
        {
            return luaState;
        }

        public void Push()
        {
            luaState.Push(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual int GetReference()
        {
            return reference;
        }

        public override bool Equals(object o)
        {
            if (o == null) return reference <= 0;
            LuaBaseRef lr = o as LuaBaseRef;
            return lr != null && lr.reference == reference;                        
        }

        static bool CompareRef(LuaBaseRef a, LuaBaseRef b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            object l = a;
            object r = b;

            if (l == null && r != null)
            {
                return r == null || b.reference <= 0;
            }

            if (l != null && r == null)
            {
                return a.reference <= 0;
            }

            return a.reference == b.reference;
        }

        public static bool operator == (LuaBaseRef a, LuaBaseRef b)
        {
            return CompareRef(a, b);
        }

        public static bool operator != (LuaBaseRef a, LuaBaseRef b)
        {
            return !CompareRef(a, b);
        }

        public bool IsAlive()
        {
            return !beDisposed;
        }
    }
}