using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace reWASDUI.UIBehavior
{
	public class IntegerTextBoxBehavior : Behavior<TextBox>
	{
		public string RegularExpression
		{
			get
			{
				return (string)base.GetValue(IntegerTextBoxBehavior.RegularExpressionProperty);
			}
			set
			{
				base.SetValue(IntegerTextBoxBehavior.RegularExpressionProperty, value);
			}
		}

		public int MaxLength
		{
			get
			{
				return (int)base.GetValue(IntegerTextBoxBehavior.MaxLengthProperty);
			}
			set
			{
				base.SetValue(IntegerTextBoxBehavior.MaxLengthProperty, value);
			}
		}

		public string EmptyValue
		{
			get
			{
				return (string)base.GetValue(IntegerTextBoxBehavior.EmptyValueProperty);
			}
			set
			{
				base.SetValue(IntegerTextBoxBehavior.EmptyValueProperty, value);
			}
		}

		public int MaxValue
		{
			get
			{
				return (int)base.GetValue(IntegerTextBoxBehavior.MaxValueProperty);
			}
			set
			{
				base.SetValue(IntegerTextBoxBehavior.MaxValueProperty, value);
			}
		}

		public int MinValue
		{
			get
			{
				return (int)base.GetValue(IntegerTextBoxBehavior.MinValueProperty);
			}
			set
			{
				base.SetValue(IntegerTextBoxBehavior.MinValueProperty, value);
			}
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			base.AssociatedObject.PreviewTextInput += this.PreviewTextInputHandler;
			base.AssociatedObject.PreviewKeyDown += this.PreviewKeyDownHandler;
			DataObject.AddPastingHandler(base.AssociatedObject, new DataObjectPastingEventHandler(this.PastingHandler));
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			base.AssociatedObject.PreviewTextInput -= this.PreviewTextInputHandler;
			base.AssociatedObject.PreviewKeyDown -= this.PreviewKeyDownHandler;
			DataObject.RemovePastingHandler(base.AssociatedObject, new DataObjectPastingEventHandler(this.PastingHandler));
		}

		private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
		{
			string text;
			if (base.AssociatedObject.Text.Length < base.AssociatedObject.CaretIndex)
			{
				text = base.AssociatedObject.Text;
			}
			else
			{
				string text2;
				text = (this.TreatSelectedText(out text2) ? text2.Insert(base.AssociatedObject.SelectionStart, e.Text) : base.AssociatedObject.Text.Insert(base.AssociatedObject.CaretIndex, e.Text));
			}
			e.Handled = !this.ValidateText(text);
		}

		private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
		{
			if (string.IsNullOrEmpty(this.EmptyValue))
			{
				return;
			}
			string text = null;
			if (e.Key == Key.Back)
			{
				if (!this.TreatSelectedText(out text) && base.AssociatedObject.SelectionStart > 0)
				{
					text = base.AssociatedObject.Text.Remove(base.AssociatedObject.SelectionStart - 1, 1);
				}
			}
			else if (e.Key == Key.Delete && !this.TreatSelectedText(out text) && base.AssociatedObject.Text.Length > base.AssociatedObject.SelectionStart)
			{
				text = base.AssociatedObject.Text.Remove(base.AssociatedObject.SelectionStart, 1);
			}
			if (text == string.Empty)
			{
				base.AssociatedObject.Text = this.EmptyValue;
				if (e.Key == Key.Back)
				{
					TextBox associatedObject = base.AssociatedObject;
					int selectionStart = associatedObject.SelectionStart;
					associatedObject.SelectionStart = selectionStart + 1;
				}
				e.Handled = true;
			}
		}

		private void PastingHandler(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(DataFormats.Text))
			{
				string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text));
				if (!this.ValidateText(text) || ((TextBox)e.Source).Text.Length - ((TextBox)e.Source).SelectedText.Length + text.Length > this.MaxLength)
				{
					e.CancelCommand();
					return;
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		private bool ValidateText(string text)
		{
			int num;
			return int.TryParse(text, out num) && (new Regex(this.RegularExpression, RegexOptions.IgnoreCase).IsMatch(text) && (this.MaxLength == int.MinValue || text.Length <= this.MaxLength) && num >= this.MinValue) && num <= this.MaxValue;
		}

		private bool TreatSelectedText(out string text)
		{
			text = null;
			if (base.AssociatedObject.SelectionLength <= 0)
			{
				return false;
			}
			int length = base.AssociatedObject.Text.Length;
			if (base.AssociatedObject.SelectionStart >= length)
			{
				return true;
			}
			if (base.AssociatedObject.SelectionStart + base.AssociatedObject.SelectionLength >= length)
			{
				base.AssociatedObject.SelectionLength = length - base.AssociatedObject.SelectionStart;
			}
			text = base.AssociatedObject.Text.Remove(base.AssociatedObject.SelectionStart, base.AssociatedObject.SelectionLength);
			return true;
		}

		public static readonly DependencyProperty RegularExpressionProperty = DependencyProperty.Register("RegularExpression", typeof(string), typeof(IntegerTextBoxBehavior), new FrameworkPropertyMetadata(".*"));

		public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(IntegerTextBoxBehavior), new FrameworkPropertyMetadata(int.MinValue));

		public static readonly DependencyProperty EmptyValueProperty = DependencyProperty.Register("EmptyValue", typeof(string), typeof(IntegerTextBoxBehavior), null);

		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(IntegerTextBoxBehavior), new PropertyMetadata(int.MaxValue));

		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(IntegerTextBoxBehavior), new PropertyMetadata(int.MinValue));
	}
}
