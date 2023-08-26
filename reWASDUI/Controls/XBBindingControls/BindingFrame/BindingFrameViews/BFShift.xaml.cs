using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Commands;
using Prism.Regions;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.XB;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace reWASDUI.Controls.XBBindingControls.BindingFrame.BindingFrameViews
{
	public partial class BFShift : BaseBFView, INavigationAware
	{
		public Type BindingFrameViewTypeToReturnBack
		{
			get
			{
				return (Type)base.GetValue(BFShift.BindingFrameViewTypeToReturnBackProperty);
			}
			set
			{
				base.SetValue(BFShift.BindingFrameViewTypeToReturnBackProperty, value);
			}
		}

		public XBBinding XBBinding
		{
			get
			{
				return (XBBinding)base.GetValue(BFShift.XBBindingProperty);
			}
			set
			{
				base.SetValue(BFShift.XBBindingProperty, value);
			}
		}

		private static void XBBindingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BFShift bfshift = d as BFShift;
			if (bfshift == null)
			{
				return;
			}
			bfshift.OnXBBindingChanged(e);
		}

		protected virtual void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			XBBinding xbbinding = e.OldValue as XBBinding;
			if (xbbinding != null)
			{
				xbbinding.MaskConditions.CollectionChanged -= this.MaskConditions_CollectionChanged;
			}
			XBBinding xbbinding2 = e.NewValue as XBBinding;
			if (xbbinding2 != null)
			{
				xbbinding2.MaskConditions.CollectionChanged += this.MaskConditions_CollectionChanged;
			}
		}

		private void MaskConditions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					XBBinding xbbinding = this.XBBinding;
					bool flag;
					if (xbbinding == null)
					{
						flag = false;
					}
					else
					{
						ObservableCollection<AssociatedControllerButton> maskConditions = xbbinding.MaskConditions;
						int? num = ((maskConditions != null) ? new int?(maskConditions.Count) : null);
						int num2 = 0;
						flag = (num.GetValueOrDefault() == num2) & (num != null);
					}
					if (flag)
					{
						goto IL_67;
					}
				}
				XBBinding xbbinding2 = this.XBBinding;
				if (xbbinding2 == null || !xbbinding2.MaskConditionsHasZones)
				{
					return;
				}
				IL_67:
				this.ShiftBack();
			}
		}

		public DelegateCommand ShiftBackCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._ShiftBack) == null)
				{
					delegateCommand = (this._ShiftBack = new DelegateCommand(new Action(this.ShiftBack)));
				}
				return delegateCommand;
			}
		}

		public BFShift()
		{
			this.InitializeComponent();
			base.Loaded += this.OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this._bindingFrameUc == null)
			{
				this._bindingFrameUc = VisualTreeHelperEx.FindAncestorByType<BindingFrameUC>(this);
			}
			if (!this._isFirstLoadInited)
			{
				this._isFirstLoadInited = true;
				if (this.XBBinding == null)
				{
					BindingOperations.SetBinding(this, BFShift.XBBindingProperty, new Binding("GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding"));
				}
			}
		}

		private void ShiftBack()
		{
			if (this.BindingFrameViewTypeToReturnBack != null)
			{
				this._bindingFrameUc.RegionManager.RequestNavigate(this.BindingFrameViewTypeToReturnBack.ToString(), null);
				return;
			}
			this._bindingFrameUc.RegionManager.NavigateToDefaultView();
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			this.BindingFrameViewTypeToReturnBack = navigationContext.Parameters["BindingFrameViewTypeToReturnBack"] as Type;
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			this.BindingFrameViewTypeToReturnBack = null;
		}

		private void RemoveJumpToShiftCommand(object sender, RoutedEventArgs e)
		{
			App.GameProfilesService.RealCurrentBeingMappedBindingCollection.CurrentXBBinding.CurrentActivatorXBBinding.ClearJumpToShift();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((SVGButton)target).Click += this.RemoveJumpToShiftCommand;
			}
		}

		public static readonly DependencyProperty BindingFrameViewTypeToReturnBackProperty = DependencyProperty.Register("BindingFrameViewTypeToReturnBack", typeof(Type), typeof(BFShift), new PropertyMetadata(null));

		public static readonly DependencyProperty XBBindingProperty = DependencyProperty.Register("XBBinding", typeof(XBBinding), typeof(BFShift), new PropertyMetadata(null, new PropertyChangedCallback(BFShift.XBBindingChangedCallback)));

		private DelegateCommand _ShiftBack;

		private BindingFrameUC _bindingFrameUc;

		private bool _isFirstLoadInited;
	}
}
