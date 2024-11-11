using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using TwoFactorAuthenticator.Security;

namespace TwoFactorAuthenticator.Presentation;

// ReSharper disable once UnusedType.Global
/// <summary>
/// Interaktionslogik für FactorControl.xaml
/// </summary>
public class FactorControl : Control
{
    #region Fields

    private byte[] _digits = new byte[6];
    private int _count;
    private int _caretPos;

    private Caret _caret;

    #endregion

    public FactorControl()
    {
        this.FocusVisualStyle = null;
        this.Focusable = true;
        this.IsHitTestVisible = true;

        this.UpdateToken();
    }

    #region Dependency properties

    private static readonly DependencyProperty AllowCopyToClipboardProperty =
        DependencyProperty.Register(nameof(AllowCopyToClipboard), typeof(bool),
            typeof(FactorControl), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether copying the token text to the clipboard is allowed. 
    /// </summary>
    [Bindable(true), Category("Behavior")]
    public bool AllowCopyToClipboard
    {
        get => (bool)this.GetValue(AllowCopyToClipboardProperty);
        set => this.SetValue(AllowCopyToClipboardProperty, value);
    }

    private static readonly DependencyProperty PasteFromClipboardModeProperty =
        DependencyProperty.Register(nameof(PasteFromClipboardMode), typeof(PasteFromClipboardMode),
            typeof(FactorControl), new FrameworkPropertyMetadata(PasteFromClipboardMode.PasteFullToken));

    /// <summary>
    /// Gets or sets the mode of how to handle paste of clipboard data.
    /// </summary>
    [Bindable(true), Category("Behavior")]
    public PasteFromClipboardMode PasteFromClipboardMode
    {
        get => (PasteFromClipboardMode)this.GetValue(PasteFromClipboardModeProperty);
        set => this.SetValue(PasteFromClipboardModeProperty, value);
    }

    private static readonly DependencyProperty TokenLengthProperty =
        DependencyProperty.Register(nameof(TokenLength), typeof(int), typeof(FactorControl),
            new FrameworkPropertyMetadata(6,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// The length of the token, defaults to 6
    /// </summary>
    [Bindable(true), Category("Data")]
    public int TokenLength
    {
        get => (int)this.GetValue(TokenLengthProperty);
        set
        {
            if (this.TokenLength != value || _digits == null)
            {
                _digits = new byte[value];
                this.UpdateToken();
            }

            this.SetValue(TokenLengthProperty, value);

            this.InvalidateVisual();
        }
    }

    private static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(int),
            typeof(FactorControl), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// The corner radius of a digit box.
    /// </summary>
    [Bindable(true), Category("Appearance")]
    public int CornerRadius
    {
        get => (int)this.GetValue(CornerRadiusProperty);
        set => this.SetValue(CornerRadiusProperty, value);
    }

    private static readonly DependencyProperty BoxBorderBrushProperty =
        DependencyProperty.Register(nameof(BoxBorderBrush), typeof(Brush),
            typeof(FactorControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// The color of the border of a digit box.
    /// </summary>
    [Bindable(true), Category("Appearance")]
    public new Brush BoxBorderBrush
    {
        get => (Brush)this.GetValue(BoxBorderBrushProperty);
        set => this.SetValue(BoxBorderBrushProperty, value);
    }

    private static readonly DependencyProperty BoxSizeProperty =
        DependencyProperty.Register(nameof(BoxSize), typeof(Size), typeof(FactorControl),
            new FrameworkPropertyMetadata(new Size(25, 25),
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// The size of each individual digit box.
    /// </summary>
    [Bindable(true), Category("Appearance")]
    public Size BoxSize
    {
        get => (Size)this.GetValue(BoxSizeProperty);
        set => this.SetValue(BoxSizeProperty, value);
    }

    private static readonly DependencyProperty BoxGapProperty =
        DependencyProperty.Register(nameof(BoxGap), typeof(int), typeof(FactorControl),
            new FrameworkPropertyMetadata(5,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// The gap between two digit boxes.
    /// </summary>
    [Bindable(true), Category("Appearance")]
    public int BoxGap
    {
        get => (int)this.GetValue(BoxGapProperty);
        set => this.SetValue(BoxGapProperty, value);
    }

    public static readonly DependencyProperty PasswordTokenProperty =
        DependencyProperty.Register(nameof(PasswordToken), typeof(PasswordToken), typeof(FactorControl),
            new PropertyMetadata());


    /// <summary>
    /// The resulting password token as entered by the user.
    /// </summary>
    /// <remarks>use Mode=OneWayToSource with Binding</remarks>
    [Bindable(true), Category("Data")]
    public PasswordToken PasswordToken
    {
        get => (PasswordToken)this.GetValue(PasswordTokenProperty);
        // ReSharper disable once ValueParameterNotUsed
        set { }
    }

    #endregion

    #region Methods

    private void UpdateToken()
    {
        var result = new PasswordToken();
        for (int i = 0; i < _count; i++)
        {
            result.AppendDigit(_digits[i]);
        }

        this.SetValue(PasswordTokenProperty, result);
    }

    private void MoveCaret()
    {
        Point pos = this.BoxPos(_caretPos);
        pos.Offset(3, this.BoxSize.Height - 3);

        var parent = VisualTreeHelper.GetParent(this) as UIElement;
        pos = this.TranslatePoint(pos, parent);

        _caret.Location = pos;
    }

    private Point BoxPos(int index)
    {
        if (index > this.TokenLength - 1) index = this.TokenLength - 1;
        if (index < 0) index = 0;

        double reqWidth = this.TokenLength * this.BoxSize.Width + (this.TokenLength - 1) * this.BoxGap;
        double posX = (this.ActualWidth - reqWidth) / 2;
        double posY = (this.ActualHeight - this.BoxSize.Height) / 2;

        if (index > 0)
        {
            posX += index * (this.BoxSize.Width + this.BoxGap);
        }

        return new Point(posX, posY);
    }

    private void InsertDigit(byte digit)
    {
        if (_count >= this.TokenLength)
        {
            SystemSounds.Beep.Play();
            return;
        }

        if (_caretPos < _count)
        {
            for (int i = _count; i > _caretPos; i--)
            {
                _digits[i] = _digits[i - 1];
            }
        }

        _digits[_caretPos] = digit;
        if (_caretPos < this.TokenLength - 1) _caretPos++;
        _count++;

        this.MoveCaret();
        this.InvalidateVisual();
        this.UpdateToken();
    }

    private void RemoveDigit()
    {
        if (_caretPos >= _count || _count <= 0)
        {
            SystemSounds.Beep.Play();
            return;
        }

        if (_caretPos < _count)
        {
            for (int i = _caretPos + 1; i < _count; i++)
            {
                _digits[i - 1] = _digits[i];
            }
        }

        _count--;
        _digits[_count] = 255;

        this.MoveCaret();
        this.InvalidateVisual();
        this.UpdateToken();
    }

    #endregion

    #region Overrides

    // protected override Visual GetVisualChild(int index) =>
    //     index == 0 ? _caret : throw new ArgumentOutOfRangeException();

    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        if (!this.IsFocused)
        {
            this.InvalidateVisual();
        }

        base.OnMouseEnter(e);
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        if (!this.IsFocused)
        {
            this.InvalidateVisual();
        }

        base.OnMouseLeave(e);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            this.InvalidateVisual();
            this.Focus();
        }

        base.OnMouseLeftButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnGotFocus(RoutedEventArgs e)
    {
        if (_caret == null)
        {
            _caret = new Caret { CaretWidth = this.BoxSize.Width - 6, CaretMode = CaretMode.Horizontal };
            var parent = VisualTreeHelper.GetParent(this);
            if (parent is IAddChild host)
            {
                host.AddChild(_caret);
            }
        }

        this.MoveCaret();
        _caret.Show();

        this.InvalidateVisual();

        base.OnGotFocus(e);
    }

    /// <inheritdoc />
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        _caret?.Hide();

        this.InvalidateVisual();

        base.OnLostFocus(e);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case >= Key.D0 and <= Key.D9:
                this.InsertDigit((byte)(e.Key - Key.D0));
                e.Handled = true;
                return;
            case >= Key.NumPad0 and <= Key.NumPad9:
                this.InsertDigit((byte)(e.Key - Key.NumPad0));
                e.Handled = true;
                return;
            case Key.Delete:
                this.RemoveDigit();
                e.Handled = true;
                return;
            case Key.Back:
                if (_caretPos > 0)
                {
                    _caretPos--;
                    this.RemoveDigit();
                    e.Handled = true;
                    return;
                }

                SystemSounds.Beep.Play();
                e.Handled = true;
                return;
            case Key.Left:
                if (_caretPos > 0)
                {
                    _caretPos--;
                    this.MoveCaret();
                    this.InvalidateVisual();
                }

                e.Handled = true;
                return;
            case Key.Right:
                if (_caretPos < _count && _caretPos < this.TokenLength - 1)
                {
                    _caretPos++;
                    this.MoveCaret();
                    this.InvalidateVisual();
                }

                e.Handled = true;
                return;
        }

        base.OnKeyDown(e);
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        return this.TokenLength == 0
                   ? new Size(0, this.BoxSize.Height + 1)
                   : new Size(this.TokenLength * this.BoxSize.Width + (this.TokenLength - 1) * this.BoxGap + 1, this.BoxSize.Height + 1);
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext dc)
    {
        double reqWidth = this.TokenLength * this.BoxSize.Width + (this.TokenLength - 1) * this.BoxGap;

        Point location = new Point((this.ActualWidth - reqWidth) / 2,
            (this.ActualHeight - this.BoxSize.Height) / 2);

        Pen border = new Pen(this.BoxBorderBrush, 1);
        Pen highlight = new Pen(SystemColors.HighlightBrush, 1);
        Brush hover = new SolidColorBrush(SystemColors.HotTrackColor) { Opacity = 0.5 } ;

        for (int i = 0; i < this.TokenLength; i++)
        {
            Brush back = this.IsMouseOver && !this.IsFocused ? hover : this.Background;
            Pen pen = this.IsFocused && i == _caretPos ? highlight : border;
            dc.DrawRoundedRectangle(back, pen, new Rect(location, this.BoxSize), this.CornerRadius, this.CornerRadius);

            location.Offset(this.BoxSize.Width + this.BoxGap, 0);
        }

        for (int i = 0; i < _count; i++)
        {
            var text = new FormattedText(_digits[i].ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                this.FontFamily.GetTypefaces().First(), this.FontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            Point pos = this.BoxPos(i);
            pos.X += (int)Math.Round((this.BoxSize.Width - text.Width) / 2);
            pos.Y += (int)Math.Round((this.BoxSize.Height - text.Height) / 2);

            dc.DrawText(text, pos);
        }
    }

    #endregion
}