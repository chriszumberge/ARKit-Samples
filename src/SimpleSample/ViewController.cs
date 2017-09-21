using ARKit;
using CoreGraphics;
using SceneKit;
using SimpleSample.Classes;
using SimpleSample.Extensions;
using System;
using System.Linq;
using UIKit;

namespace SimpleSample
{
    public partial class ViewController : UIViewController, IARSCNViewDelegate
    {
        public ARSCNView SceneView
        {
            get { return View as ARSCNView; }
        }

        public float AmbientIntensity
        {
            get
            {
                // Get the current frame
                var frame = SceneView.Session.CurrentFrame;
                if (frame == null) return 1000;

                // Return ambient intensity
                if (frame.LightEstimate == null)
                {
                    return 1000;
                }
                else
                {
                    return (float)frame.LightEstimate.AmbientIntensity;
                }
            }
        }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            // Set self as Scene View's delegate
            SceneView.Delegate = this;

            // Track changes to the session
            SceneView.Session.Delegate = new SessionDelegate();

            // Create a new scene
            var scene = SCNScene.FromFile("art.scnassets/ship");

            // Set the scene to the view
            SceneView.Scene = scene;

            // Add a tap gesture recognizer
            var tapGesture = new UITapGestureRecognizer(HandleTap);
            View.AddGestureRecognizer(tapGesture);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Create a session configuration
            var configuration = new ARWorldTrackingConfiguration
            {
                PlaneDetection = ARPlaneDetection.Horizontal,
                LightEstimationEnabled = true
            };

            // Run the view's session
            SceneView.Session.Run(configuration, ARSessionRunOptions.ResetTracking);

            // FInd the ship and position it just in front of the camera
            var ship = SceneView.Scene.RootNode.FindChildNode("ship", true);

            ship.Position = new SCNVector3(2f, -2f, -9f);

            //HACK: to see the jet move (circle around the viewer in a roll), comment out the ship.Position line above
            // and uncomment the code below (courtesy @lobrien)

            //var animation = SCNAction.RepeatActionForever(SCNAction.RotateBy(0, (float)Math.PI, (float)Math.PI, (float)1));
            //var pivotNode = new SCNNode { Position = new SCNVector3(0.0f, 2.0f, 0.0f) };
            //pivotNode.RunAction(SCNAction.RepeatActionForever(SCNAction.RotateBy(0, -2, 0, 10)));
            //ship.RemoveFromParentNode();
            //pivotNode.AddChildNode(ship);
            //SceneView.Scene.RootNode.AddChildNode(pivotNode);
            //ship.Scale = new SCNVector3(0.1f, 0.1f, 0.1f);
            //ship.Position = new SCNVector3(2f, -2f, -3f);
            //ship.RunAction(SCNAction.RepeatActionForever(SCNAction.RotateBy(0, 0, 2, 1)));

            //ENDHACK
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Puase the view's session
            SceneView.Session.Pause();
        }

        public override bool ShouldAutorotate()
        {
            //return base.ShouldAutorotate();
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            //return base.GetSupportedInterfaceOrientations();
            return UIInterfaceOrientationMask.All;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void HandleTap()
        {
            // Get the current frame
            var currentFrame = SceneView.Session.CurrentFrame;
            if (currentFrame == null) return;

            // Create an image plane using a snapshot of the view
            var imagePlane = SCNPlane.Create(SceneView.Bounds.Width / 6000, SceneView.Bounds.Height / 6000);
            imagePlane.FirstMaterial.Diffuse.Contents = SceneView.Snapshot();
            imagePlane.FirstMaterial.LightingModelName = SCNLightingModel.Constant;

            // Create a plane node and add it to the scene
            var planeNode = SCNNode.FromGeometry(imagePlane);
            SceneView.Scene.RootNode.AddChildNode(planeNode);

            // Set transform of node to be 10cm in front of the camera
            var translation = SCNMatrix4.CreateTranslation(0, 0, 0.1f);
            var cameraTranslation = currentFrame.Camera.Transform.ToSCNMatrix4();
            planeNode.Transform = SCNMatrix4.Mult(cameraTranslation, translation);
        }

        private void AddAnchorToScene()
        {
            // Get the current frame
            var frame = SceneView.Session.CurrentFrame;
            if (frame == null) return;

            // Create a ray to test from 
            var point = new CGPoint(0.5, 0.5);

            // Perform hit testing on frame
            var results = frame.HitTest(point, ARHitTestResultType.ExistingPlane | ARHitTestResultType.EstimatedHorizontalPlane);

            // Use the first result
            if (results.Length > 0)
            {
                var result = results.First();

                // Create an anchor for it
                var anchor = new ARAnchor(result.WorldTransform);

                // Add anchor to session
                SceneView.Session.AddAnchor(anchor);
            }
        }
    }
}