//////////////////////////////////////////////
// References:
//      https://www.codeproject.com/tips/431000/Caret-for-WPF-User-Controls
//      https://github.com/abbaye/WpfCaret
//////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TwoFactorAuthenticator.Presentation;

/// <summary>
///     A FrameworkElement representing a blinking caret.
/// </summary>
internal sealed class Caret : FrameworkElement
{
    #region Fields

    private DispatcherTimer _timer;
    private Pen _pen;
    private Brush _brush;
    private bool _drawCaret;

    #endregion

    #region Constructor

    public Caret()
    {
        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            this.Init(Brushes.Black);
        }
    }

    public Caret(Brush brush) : this()
    {
        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            this.Init(brush);
        }
    }

    private void Init(Brush brush)
    {
        _pen = new Pen(brush, 1.0);
        _pen.Freeze();
        _brush = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };
        _brush.Freeze();

        this.IsHitTestVisible = false;

        _timer = new DispatcherTimer((TimeSpan)BlinkIntervalProperty.DefaultMetadata.DefaultValue, DispatcherPriority.Normal, this.BlinkCaret, this.Dispatcher);
    }

    #endregion

    #region Dependency properties

    private static readonly DependencyProperty CaretVisibleProperty =
        DependencyProperty.Register(nameof(CaretVisible), typeof(bool),
            typeof(Caret), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets a value indicating whether the caret is visible.
    /// </summary>
    public bool CaretVisible
    {
        get => (bool)this.GetValue(CaretVisibleProperty);
        set
        {
            if (this.CaretVisible == value)
            {
                return;
            }

            _drawCaret = value;
            this.SetValue(CaretVisibleProperty, value);
            this.InvalidateVisual();

            if (value)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }
    }

    private static readonly DependencyProperty CaretHeightProperty =
        DependencyProperty.Register(nameof(CaretHeight), typeof(double),
            typeof(Caret), new FrameworkPropertyMetadata(18d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the height of the caret.
    /// </summary>
    /// <remarks>
    /// Height is relevant for Vertical and Block mode. 
    /// </remarks>
    public double CaretHeight
    {
        get => (double)this.GetValue(CaretHeightProperty);
        set => this.SetValue(CaretHeightProperty, value);
    }

    private static readonly DependencyProperty CaretWidthProperty =
        DependencyProperty.Register(nameof(CaretWidth), typeof(double),
            typeof(Caret), new FrameworkPropertyMetadata(9d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the width of the caret.
    /// </summary>
    /// <remarks>
    /// Height is relevant for Horizontal and Block mode. 
    /// </remarks>
    public double CaretWidth
    {
        get => (double)this.GetValue(CaretWidthProperty);
        set => this.SetValue(CaretWidthProperty, value);
    }

    private static readonly DependencyProperty CaretLocationProperty =
        DependencyProperty.Register(nameof(Location), typeof(Point),
            typeof(Caret), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the location of the caret.
    /// </summary>
    public Point Location
    {
        get => (Point)this.GetValue(CaretLocationProperty);
        set
        {
            if (this.CaretVisible)
            {
                _drawCaret = true;
            }
            this.SetValue(CaretLocationProperty, value);
            this.InvalidateVisual();
        }
    }

    private static readonly DependencyProperty BlinkIntervalProperty =
        DependencyProperty.Register(nameof(BlinkInterval), typeof(TimeSpan),
            typeof(Caret), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(600), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the blinking interval.
    /// </summary>
    public TimeSpan BlinkInterval
    {
        get => (TimeSpan)this.GetValue(BlinkIntervalProperty);
        set
        {
            this.SetValue(BlinkIntervalProperty, value);
            _timer.Interval = value;
        }
    }

    private static readonly DependencyProperty CaretModeProperty =
        DependencyProperty.Register(nameof(CaretMode), typeof(CaretMode),
            typeof(Caret), new FrameworkPropertyMetadata(CaretMode.Vertical));

    /// <summary>
    /// Gets or sets the caret display mode.
    /// </summary>
    public CaretMode CaretMode
    {
        get => (CaretMode)this.GetValue(CaretModeProperty);
        set
        {
            this.SetValue(CaretModeProperty, value);
            this.InvalidateVisual();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Convenience method to hide the caret
    /// </summary>
    public void Hide()
    {
        this.CaretVisible = false;
    }

    /// <summary>
    /// Convenience method to show the caret
    /// </summary>
    public void Show()
    {
        this.CaretVisible = true;
    }

    /// <summary>
    /// Timer delegate for blinking the caret
    /// </summary>
    private void BlinkCaret(object state, EventArgs eventArgs)
    {
        try
        {
            this.Dispatcher?.Invoke(() =>
                                    {
                                        _drawCaret = !_drawCaret;
                                        this.InvalidateVisual();
                                    });
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Render the caret
    /// </summary>
    protected override void OnRender(DrawingContext dc)
    {
        if (!this.CaretVisible || !_drawCaret)
        {
            return;
        }

        switch (this.CaretMode)
        {
            case CaretMode.Vertical:
                dc.DrawLine(_pen, this.Location, new Point(this.Location.X, this.Location.Y + this.CaretHeight));
                break;
            case CaretMode.Horizontal:
                dc.DrawLine(_pen, this.Location, new Point(this.Location.X + this.CaretWidth, this.Location.Y));
                break;
            case CaretMode.Block:
                dc.DrawRectangle(_brush, _pen, new Rect(this.Location.X, this.Location.Y, this.CaretWidth, this.CaretHeight));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}