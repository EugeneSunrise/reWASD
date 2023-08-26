using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DiscSoft.NET.Common.Utils.ExtensionMethods;
using DiscSoft.NET.Common.View.Controls.Buttons;
using Prism.Commands;
using reWASDUI.Infrastructure;
using reWASDUI.Infrastructure.KeyBindings;
using reWASDUI.Infrastructure.KeyBindings.Mask;
using reWASDUI.Infrastructure.KeyBindings.XB;
using reWASDUI.Properties;
using reWASDUI.Services;
using reWASDUI.Views;

namespace reWASDUI.Controls.XBBindingControls.BindingAnnotation
{
	public partial class GamepadMaskAnnotation : BaseXBBindingAnnotation, INotifyPropertyChanged
	{
		public ICollectionView FilteredMasksForAnnotations
		{
			get
			{
				return this._filteredMasksForAnnotations;
			}
			set
			{
				this.SetProperty(ref this._filteredMasksForAnnotations, value, "FilteredMasksForAnnotations");
			}
		}

		public bool IsMaskVisible
		{
			get
			{
				return (bool)base.GetValue(GamepadMaskAnnotation.IsMaskVisibleProperty);
			}
			set
			{
				base.SetValue(GamepadMaskAnnotation.IsMaskVisibleProperty, value);
			}
		}

		public DelegateCommand NavigateToMaskWithFilterCommand
		{
			get
			{
				DelegateCommand delegateCommand;
				if ((delegateCommand = this._navigateToMaskWithFilter) == null)
				{
					delegateCommand = (this._navigateToMaskWithFilter = new DelegateCommand(new Action(this.NavigateToMaskWithFilter)));
				}
				return delegateCommand;
			}
		}

		private void NavigateToMaskWithFilter()
		{
			base.XBBinding.HostCollection.MaskBindingCollection.AssociatedControllerButtonContainer.SetButtonsFromAnotherInstance(base.XBBinding.ControllerButton);
			reWASDApplicationCommands.NavigateGamepadCommand.Execute(typeof(MaskView));
		}

		public GamepadMaskAnnotation()
		{
			this.InitializeComponent();
		}

		protected override void ReEvaluateXBBinding()
		{
		}

		protected override void ReEvaluateVisibility()
		{
			bool flag;
			if (base.XBBinding != null)
			{
				GameProfilesService gameProfilesService = base.GameProfilesService;
				if (gameProfilesService == null)
				{
					flag = false;
				}
				else
				{
					BaseXBBindingCollection realCurrentBeingMappedBindingCollection = gameProfilesService.RealCurrentBeingMappedBindingCollection;
					bool? flag2 = ((realCurrentBeingMappedBindingCollection != null) ? new bool?(realCurrentBeingMappedBindingCollection.MaskBindingCollection.Any(new Func<MaskItem, bool>(this.Filter))) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
			}
			else
			{
				flag = false;
			}
			this.IsMaskVisible = flag;
		}

		protected override void ReEvaluateIsCurrentBinding()
		{
		}

		protected override void OnXBBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnXBBindingChanged(e);
			this.RecreateFilteredSource();
		}

		private void RecreateFilteredSource()
		{
			if (base.XBBinding == null)
			{
				return;
			}
			ICollectionView defaultView = CollectionViewSource.GetDefaultView(base.XBBinding.HostCollection.MaskBindingCollection);
			defaultView.Filter = new Predicate<object>(this.Filter);
			this.FilteredMasksForAnnotations = defaultView;
		}

		private bool Filter(object obj)
		{
			return this.Filter(obj as MaskItem);
		}

		private bool Filter(MaskItem mi)
		{
			if (mi != null)
			{
				return mi.MaskConditions.All((MaskItemCondition mic) => mic.ControllerButton.IsGamepad || mic.ControllerButton.IsKeyScanCode || mic.IsNotSelected) && mi.MaskConditions.Any(delegate(MaskItemCondition mic)
				{
					if (mic.ControllerButton.IsSet)
					{
						AssociatedControllerButton controllerButton = mic.ControllerButton;
						XBBinding xbbinding = base.XBBinding;
						if (controllerButton.IsAssociatedSetToEqualButtons((xbbinding != null) ? xbbinding.ControllerButton : null))
						{
							return (int)mic.ControllerFamilyIndex == base.XBBinding.HostCollection.SubConfigData.IndexByControllerFamily;
						}
					}
					return false;
				});
			}
			return false;
		}

		private void FrameworkElement_OnToolTipOpening(object sender, ToolTipEventArgs e)
		{
			this.RecreateFilteredSource();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "7.0.5.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((SVGButton)target).ToolTipOpening += this.FrameworkElement_OnToolTipOpening;
			}
		}

		private ICollectionView _filteredMasksForAnnotations;

		public static readonly DependencyProperty IsMaskVisibleProperty = DependencyProperty.Register("IsMaskVisible", typeof(bool), typeof(GamepadMaskAnnotation), new PropertyMetadata(false));

		private DelegateCommand _navigateToMaskWithFilter;
	}
}
