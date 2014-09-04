using ModernWindowFrameLibrary.Support;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModernWindowFrameLibrary
{
    public class ModernWindowFrame : Window
    {
        protected Button[] headerButtons;
        protected Border   windowHeader;

        protected bool     resizable;
        protected double[] windowDimentions;

        private Rectangle[] windowEdges;
        private ResizeGrip  resizeGrip;

        private bool         windowMaximized;
        private bool         mouseCaptured;
        private bool         windowMovable;
        private Point        mouseStaticIntrnPos;
        private resizeHandle currentWindowResizeHandler;
        private resizeHandle resizeHandlerBeforeMaximize;

        public ModernWindowFrame()
        {
            headerButtons = new Button[3];
            windowEdges   = new Rectangle[8];

            mouseCaptured = false;
            Cursor        = Cursors.Arrow;

            replaceDefaultWindowChrome();

            ApplyTemplate();

            referenceXAMLElements();
            applySafeDefaults();
            registerEventHandlerS();
        }

        protected virtual void applySafeDefaults()
        {
            BorderThickness = new Thickness(3);
            BorderBrush     = new SolidColorBrush(Colors.Black);
            ResizeHandle    = resizeHandle.WindowEdges;
            Movable         = true;
        }

        private void attachBaseUIEventHandlers(object uIElement)
        {
            ((UIElement)uIElement).MouseLeftButtonDown += mouseLeftButtonDownEventHandler;
            ((UIElement)uIElement).MouseEnter          += mouseEnterEventHandler;
            ((UIElement)uIElement).MouseLeave          += mouseLeaveEventHandler;
            ((UIElement)uIElement).MouseMove           += mouseMoveEventHandler;
            ((UIElement)uIElement).MouseLeftButtonUp   += mouseLeftButtonUpEventHandler;
        }

        public new SolidColorBrush BorderBrush
        {
            set
            {
                int counter = 0;

                while (true)
                {
                    windowEdges[counter].Fill = value;

                    if (counter.Equals(7)) break;

                    counter++;
                }
            }

            get
            {
                return (SolidColorBrush)windowEdges[0].Fill;
            }
        }

        private double borderThickness
        {
            set
            {
                if (value < 0) throw new ArgumentException("Thickness Cannot Be A Negetive Value");

                int counter = 0;

                while (true)
                {
                    switch (counter)
                    {
                        case 0:
                            windowEdges[counter].Width  = value;
                            break;

                        case 2:
                            windowEdges[counter].Height = value;
                            break;

                        case 4:
                            windowEdges[counter].Width  = value;
                            break;

                        case 6:
                            windowEdges[counter].Height = value;
                            break;

                        default:
                            windowEdges[counter].Width  = value;
                            windowEdges[counter].Height = value;
                            break;
                    }

                    if (counter.Equals(7)) break;

                    counter++;
                }
            }

            get
            {
                return windowEdges[0].Width;
            }
        }

        public new Thickness BorderThickness
        {
            set
            {
                borderThickness = value.Left; //assumption being made that the values on all sides of the thickness are the same
            }

            get
            {
                return new Thickness(borderThickness);
            }
        }

        protected virtual void buttonCloseClickEventHandler(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected virtual void buttonMaximizeClickEventHandler(object sender, RoutedEventArgs e)
        {
            if (windowMaximized)
            {
                if (windowDimentions == null) return;

                Top    = windowDimentions[0];
                Left   = windowDimentions[1];
                Width  = windowDimentions[2];
                Height = windowDimentions[3];

                windowMaximized = false;

                ResizeHandle = resizeHandlerBeforeMaximize;
            }

            else if (!windowMaximized)
            {
                if (windowDimentions == null)
                    windowDimentions = new double[4];

                windowDimentions[0] = Top;
                windowDimentions[1] = Left;
                windowDimentions[2] = Width;
                windowDimentions[3] = Height;

                double[] bound = FormsScreenWrapper.currentDisplayAvailableBounds();

                Top    = bound[0];
                Left   = bound[1];
                Width  = bound[2];
                Height = bound[3];

                windowMaximized = true;

                resizeHandlerBeforeMaximize = currentWindowResizeHandler;
                ResizeHandle                = resizeHandle.None;
            }
        }

        protected virtual void buttonMinimizeClickEventHandler(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void referenceXAMLElements()
        {
            resizeGrip   = (ResizeGrip)GetTemplateChild("resizeGrip");
            windowHeader = (Border)GetTemplateChild("WindowHeaderBorder");

            int counter = 0;

            while (true)
            {
                if (counter < 3)
                {
                    switch (counter)
                    {
                        case 0:
                            headerButtons[counter] = (Button)GetTemplateChild("HeaderButtonMinimize");
                            break;

                        case 1:
                            headerButtons[counter] = (Button)GetTemplateChild("HeaderButtonMaximize");
                            break;

                        case 2:
                            headerButtons[counter] = (Button)GetTemplateChild("HeaderButtonClose");
                            break;
                    }
                }

                switch (counter)
                {
                    case 0:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("LeftEdge");
                        break;

                    case 1:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("TopLeftEdge");
                        break;

                    case 2:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("TopEdge");
                        break;

                    case 3:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("TopRightEdge");
                        break;

                    case 4:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("RightEdge");
                        break;

                    case 5:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("BottomRightEdge");
                        break;

                    case 6:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("BottomEdge");
                        break;

                    case 7:
                        windowEdges[counter] = (Rectangle)GetTemplateChild("BottomLeftEdge");
                        break;
                }

                if (counter.Equals(7)) break;

                counter++;
            }
        }

        private void registerEventHandlerS()
        {
            headerButtons[0].Click += buttonMinimizeClickEventHandler;
            headerButtons[1].Click += buttonMaximizeClickEventHandler;
            headerButtons[2].Click += buttonCloseClickEventHandler;

            foreach(Rectangle windowEdge in windowEdges)
            {
                attachBaseUIEventHandlers(windowEdge);
            }

            attachBaseUIEventHandlers(resizeGrip);
            attachBaseUIEventHandlers(windowHeader);
        }

        protected bool MouseCaptured
        {
            get
            {
                return mouseCaptured;
            }
        }

        private void mouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (mouseCaptured)
            {
                Point mouseIntrnPos = e.GetPosition(this);

                if (sender is Border)
                {
                    if (!((Border)sender).IsMouseCaptured) return;

                    if (!Movable) return;

                    if (windowMaximized)
                    {
                        buttonMaximizeClickEventHandler(null, null);

                        mouseStaticIntrnPos = new Point((e.GetPosition(this).Y + (this.Width / 2)), e.GetPosition(this).Y);
                    }

                    Point mouseExtrnPos = FormsScreenWrapper.mouseLocation();
                    this.Left = (mouseExtrnPos.X - mouseStaticIntrnPos.X);
                    this.Top  = (mouseExtrnPos.Y - mouseStaticIntrnPos.Y);
                }

                if (sender is Rectangle)
                {
                    if (!((Rectangle)sender).IsMouseCaptured) return;

                    if (ResizeHandle.Equals(resizeHandle.None) || ResizeHandle.Equals(resizeHandle.ResizeGrip))
                    {
                        ((Rectangle)sender).ReleaseMouseCapture();
                        mouseCaptured = false;

                        return;
                    }

                    switch (((Rectangle)sender).Name)
                    {
                        case "LeftEdge":
                            break;

                        case "TopLeftEdge":
                            break;

                        case "TopEdge":
                            break;

                        case "TopRightEdge":
                            break;

                        case "RightEdge":
                            if (mouseIntrnPos.X > MinWidth)
                                this.Width = mouseIntrnPos.X;

                            if (mouseIntrnPos.X <= MinWidth)
                                this.Width = MinWidth;
                            break;

                        case "BottomRightEdge":
                            if (mouseIntrnPos.Y > MinHeight && mouseIntrnPos.X > MinWidth)
                            {
                                this.Height = mouseIntrnPos.Y;
                                this.Width = mouseIntrnPos.X;
                            }
                            else
                            {
                                if (mouseIntrnPos.Y > MinHeight)
                                    this.Height = mouseIntrnPos.Y;

                                if (mouseIntrnPos.Y <= MinHeight)
                                    this.Height = MinHeight;

                                if (mouseIntrnPos.X > MinWidth)
                                    this.Width = mouseIntrnPos.X;

                                if (mouseIntrnPos.X <= MinWidth)
                                    this.Width = MinWidth;
                            }
                            break;

                        case "BottomEdge":
                            if (mouseIntrnPos.Y > MinHeight)
                            this.Height = mouseIntrnPos.Y;

                            if (mouseIntrnPos.Y <= MinHeight)
                                this.Height = MinHeight;
                            break;

                        case "BottomLeftEdge":
                            break;
                    }
                }

                if (sender is ResizeGrip)
                {
                    if (!((ResizeGrip)sender).IsMouseCaptured) return;

                    if (mouseIntrnPos.Y > MinHeight && mouseIntrnPos.X > MinWidth)
                    {
                        this.Height = mouseIntrnPos.Y;
                        this.Width  = mouseIntrnPos.X;
                    }
                    else
                    {
                        if (mouseIntrnPos.Y > MinHeight)
                            this.Height = mouseIntrnPos.Y;

                        if (mouseIntrnPos.Y <= MinHeight)
                            this.Height = MinHeight;

                        if (mouseIntrnPos.X > MinWidth)
                            this.Width = mouseIntrnPos.X;

                        if (mouseIntrnPos.X <= MinWidth)
                            this.Width = MinWidth;
                    }
                }
            }
        }

        private void mouseLeftButtonDownEventHandler(object sender, MouseButtonEventArgs e)
        {
            switch (mouseCaptured)
            {
                case true:
                    ((UIElement)sender).ReleaseMouseCapture();
                    mouseCaptured = false;
                    break;

                case false:
                    mouseStaticIntrnPos = e.GetPosition(this);
                    ((UIElement)sender).CaptureMouse();
                    mouseCaptured = true;
                    break;
            }

            if (e.ClickCount.Equals(2) && sender is Border)
            {
                buttonMaximizeClickEventHandler(null, null);

                mouseCaptured = false;
                e.Handled     = true;
            }
        }

        private void mouseEnterEventHandler(object sender, MouseEventArgs e)
        {
            if (sender is Rectangle) 
            {
                if (ResizeHandle.Equals(resizeHandle.All) || ResizeHandle.Equals(resizeHandle.WindowEdges))
                {
                    switch (((Rectangle)sender).Name)
                    {
                        case "LeftEdge":
                            this.Cursor = Cursors.SizeWE;
                            break;

                        case "TopLeftEdge":
                            this.Cursor = Cursors.SizeNWSE;
                            break;

                        case "TopEdge":
                            this.Cursor = Cursors.SizeNS;
                            break;

                        case "TopRightEdge":
                            this.Cursor = Cursors.SizeNESW;
                            break;

                        case "RightEdge":
                            this.Cursor = Cursors.SizeWE;
                            break;

                        case "BottomRightEdge":
                            this.Cursor = Cursors.SizeNWSE;
                            break;

                        case "BottomEdge":
                            this.Cursor = Cursors.SizeNS;
                            break;

                        case "BottomLeftEdge":
                            this.Cursor = Cursors.SizeNESW;
                            break;
                    }
                }
            }

            if (sender is ResizeGrip) { this.Cursor = Cursors.SizeNWSE; }
        }

        private void mouseLeaveEventHandler(object sender, MouseEventArgs e)
        {
            if (!this.Cursor.Equals(Cursors.Arrow)) { this.Cursor = Cursors.Arrow; }
        }

        private void mouseLeftButtonUpEventHandler(object sender, MouseButtonEventArgs e)
        {
            if (mouseCaptured) 
            {
                ((UIElement)sender).ReleaseMouseCapture();
                mouseCaptured = false;
            }
        }

        public bool Movable
        {
            protected set
            {
                windowMovable = value;
            }

            get
            {
                return windowMovable;
            }
        }

        public WindowState ActualWindowState
        {
            get
            {
                if (WindowState.Equals(WindowState.Minimized))
                {
                    return WindowState.Minimized;
                }
                else if (windowMaximized)
                {
                    return WindowState.Maximized;
                }
                else
                {
                    return WindowState.Normal;
                }
            }
        }

        private void replaceDefaultWindowChrome()
        {
            ResizeMode  = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;

            Style = (Style)TryFindResource("nWindowStyle");
        }

        public resizeHandle ResizeHandle
        {
            set
            {
                switch (value)
                {
                    case resizeHandle.All:
                        currentWindowResizeHandler = resizeHandle.All;
                        resizeGrip.Visibility = Visibility.Visible;
                        break;

                    case resizeHandle.None:
                        currentWindowResizeHandler = resizeHandle.None;
                        resizeGrip.Visibility = Visibility.Collapsed;
                        break;

                    case resizeHandle.ResizeGrip:
                        currentWindowResizeHandler = resizeHandle.ResizeGrip;
                        resizeGrip.Visibility = Visibility.Visible;
                        break;

                    case resizeHandle.WindowEdges:
                        currentWindowResizeHandler = resizeHandle.WindowEdges;
                        resizeGrip.Visibility = Visibility.Collapsed;
                        break;
                }
            }

            get
            {
                return currentWindowResizeHandler;
            }
        }

        public enum resizeHandle
        {
            All,
            WindowEdges,
            ResizeGrip,
            None
        }

        public double[] WindowDimentions
        {
            get
            {
                if (windowMaximized)
                {
                    return FormsScreenWrapper.displayAvailableBounds((int)this.Left, (int)this.Top);
                }
                else
                {
                    return windowDimentions;
                }
            }
        }

        protected Image WindowIcon
        {
            get
            {
                return (Image)GetTemplateChild("WindowIcon");
            }
        }
    }
}
