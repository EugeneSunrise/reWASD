using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prism.Regions;

namespace reWASDUI.Utils
{
	internal class CachingGridRegionAdapter : RegionAdapterBase<Grid>
	{
		public CachingGridRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
			: base(regionBehaviorFactory)
		{
		}

		protected override void Adapt(IRegion region, Grid regionTarget)
		{
			if (regionTarget == null)
			{
				throw new ArgumentNullException("regionTarget");
			}
			if (regionTarget.Children.Count > 0)
			{
				throw new InvalidOperationException("ContentControlHasContentException");
			}
			region.ActiveViews.CollectionChanged += delegate
			{
				List<UIElement> list = new List<UIElement>();
				foreach (object obj in regionTarget.Children)
				{
					UIElement uielement = (UIElement)obj;
					uielement.Visibility = Visibility.Collapsed;
					if (Attribute.GetCustomAttribute(uielement.GetType(), typeof(DoNotCacheViewAttribute)) != null)
					{
						list.Add(uielement);
					}
				}
				list.ForEach(delegate(UIElement item)
				{
					regionTarget.Children.Remove(item);
				});
				UIElement viewToActivate = region.ActiveViews.FirstOrDefault<object>() as UIElement;
				if (viewToActivate == null)
				{
					return;
				}
				UIElement uielement2 = regionTarget.Children.Cast<UIElement>().SingleOrDefault((UIElement x) => x == viewToActivate);
				if (uielement2 == null)
				{
					if (BindingOperations.GetBindingExpressionBase(regionTarget, UIElement.VisibilityProperty) != null)
					{
						throw new NotSupportedException();
					}
					regionTarget.Children.Add(viewToActivate);
					uielement2 = viewToActivate;
				}
				uielement2.Visibility = Visibility.Visible;
			};
			region.Views.CollectionChanged += delegate([Nullable(2)] object sender, NotifyCollectionChangedEventArgs e)
			{
				if (e.Action == NotifyCollectionChangedAction.Add && !region.ActiveViews.Any<object>())
				{
					region.Activate(e.NewItems[0]);
				}
			};
		}

		protected override IRegion CreateRegion()
		{
			return new SingleActiveRegion();
		}
	}
}
