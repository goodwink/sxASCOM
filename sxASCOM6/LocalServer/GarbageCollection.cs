// tabs=4
// Copyright 2010-2010 by Dad Dog Development, Ltd
//
// This work is licensed under the Creative Commons Attribution-No Derivative 
// Works 3.0 License. 
//
// A copy of the license should have been included with this software. If
// not, you can also view a copy of this license, at:
//
// http://creativecommons.org/licenses/by-nd/3.0/ or 
// send a letter to:
//
// Creative Commons
// 171 Second Street
// Suite 300
// San Francisco, California, 94105, USA.
// 
// If this license is not suitable for your purposes, it is possible to 
// obtain it under a different license. 
//
// For more information please contact bretm@daddog.com

using System;
using System.Threading;

namespace ASCOM.StarlightXpress
{
    /// <summary>
    /// Summary description for GarbageCollection.
    /// </summary>
    class GarbageCollection
    {
        protected bool m_bContinueThread;
        protected bool m_GCWatchStopped;
        protected int m_iInterval;
        protected ManualResetEvent m_EventThreadEnded;

        public GarbageCollection(int iInterval)
        {
            m_bContinueThread = true;
            m_GCWatchStopped = false;
            m_iInterval = iInterval;
            m_EventThreadEnded = new ManualResetEvent(false);
        }

        public void GCWatch()
        {
            // Pause for a moment to provide a delay to make threads more apparent.
            while (ContinueThread())
            {
                GC.Collect();
                Thread.Sleep(m_iInterval);
            }
            m_EventThreadEnded.Set();
        }

        protected bool ContinueThread()
        {
            lock (this)
            {
                return m_bContinueThread;
            }
        }

        public void StopThread()
        {
            lock (this)
            {
                m_bContinueThread = false;
            }
        }

        public void WaitForThreadToStop()
        {
            m_EventThreadEnded.WaitOne();
            m_EventThreadEnded.Reset();
        }
    }
}
