﻿using ARKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSample.Classes
{
    public class SessionDelegate : ARSessionDelegate
    {
        public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            //base.CameraDidChangeTrackingState(session, camera);

            var state = "";
            var reason = "";

            switch(camera.TrackingState)
            {
                case ARTrackingState.NotAvailable:
                    state = "Tracking Not Available";
                    break;
                case ARTrackingState.Normal:
                    state = "Tracking Normal";
                    break;
                case ARTrackingState.Limited:
                    state = "Tracking Limited";
                    switch(camera.TrackingStateReason)
                    {
                        case ARTrackingStateReason.ExcessiveMotion:
                            reason = "because of excessive motion";
                            break;
                        case ARTrackingStateReason.Initializing:
                            reason = "because tracking is initializing";
                            break;
                        case ARTrackingStateReason.InsufficientFeatures:
                            reason = "because of insufficient features in the environment";
                            break;
                        case ARTrackingStateReason.None:
                            reason = "because of an unknown reason";
                            break;
                    }
                    break;
            }

            Console.WriteLine($"{state} {reason}");
        }
    }
}
