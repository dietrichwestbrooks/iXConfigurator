using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils
{
    public static class TreeHelper
    {
        public static DependencyObject GetParent(DependencyObject element)
        {
            return GetParent(element, true);
        }

        private static DependencyObject GetParent(DependencyObject element, bool recurseIntoPopup)
        {
            if (recurseIntoPopup)
            {
                var popup = element as Popup;

                if (popup?.PlacementTarget != null)
                    return popup.PlacementTarget;
            }

            var visual = element as Visual;
            var parent = (visual == null) ? null : VisualTreeHelper.GetParent(visual);

            if (parent != null)
                return parent;

            var fe = element as FrameworkElement;

            if (fe != null)
            {
                parent = fe.Parent ?? fe.TemplatedParent;
            }
            else
            {
                var fce = element as FrameworkContentElement;

                if (fce == null)
                    return null;

                parent = fce.Parent ?? fce.TemplatedParent;
            }

            return parent;
        }

        public static T FindParent<T>(DependencyObject startingObject) 
            where T : DependencyObject
        {
            return FindParent<T>(startingObject, false, null);
        }

        public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject) 
            where T : DependencyObject
        {
            return FindParent<T>(startingObject, checkStartingObject, null);
        }

        public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject, Func<T, bool> additionalCheck) 
            where T : DependencyObject
        {
            var parent = (checkStartingObject ? startingObject : GetParent(startingObject, true));

            while (parent != null)
            {
                var foundElement = parent as T;

                if (foundElement != null)
                {
                    if (additionalCheck == null)
                    {
                        return foundElement;
                    }

                    if (additionalCheck(foundElement))
                        return foundElement;
                }

                parent = GetParent(parent, true);
            }

            return null;
        }

        public static T FindChild<T>(DependencyObject parent) 
            where T : DependencyObject
        {
            return FindChild<T>(parent, null);
        }

        public static T FindChild<T>(DependencyObject parent, Func<T, bool> additionalCheck) 
            where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            T child;

            for (var index = 0; index < childrenCount; index++)
            {
                child = VisualTreeHelper.GetChild(parent, index) as T;

                if (child != null)
                {
                    if (additionalCheck == null)
                    {
                        return child;
                    }
                    else
                    {
                        if (additionalCheck(child))
                            return child;
                    }
                }
            }

            for (var index = 0; index < childrenCount; index++)
            {
                child = FindChild(VisualTreeHelper.GetChild(parent, index), additionalCheck);

                if (child != null)
                    return child;
            }

            return null;
        }

        public static bool IsDescendantOf(DependencyObject element, DependencyObject parent)
        {
            return IsDescendantOf(element, parent, true);
        }

        public static bool IsDescendantOf(DependencyObject element, DependencyObject parent, bool recurseIntoPopup)
        {
            while (element != null)
            {
                if (Equals(element, parent))
                    return true;

                element = GetParent(element, recurseIntoPopup);
            }

            return false;
        }
    }
}
