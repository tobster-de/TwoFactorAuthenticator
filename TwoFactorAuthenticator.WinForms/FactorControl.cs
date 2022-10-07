using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TwoFactorAuthenticator.Security;

namespace TwoFactorAuthenticator.WinForms
{
    public class FactorControl : UserControl
    {
        private readonly Action<PaintEventArgs, Rectangle> _paintTransparentBackground;

        private int _caretPos, _count;
        private byte[] _digits = new byte[6];
        private int _tokenLength = 6;

        public FactorControl()
        {
            base.DoubleBuffered = true;
            this.Size = this.GetPreferredSize(new Size());

            MethodInfo method = typeof(Control).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                               .First(m => m.Name.Equals("PaintTransparentBackground") &&
                                                           m.GetParameters().Length == 2);
            _paintTransparentBackground =
                (Action<PaintEventArgs, Rectangle>)method.CreateDelegate(typeof(Action<PaintEventArgs, Rectangle>),
                    this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether copying the token text to the clipboard. 
        /// </summary>
        [EditorBrowsable]
        public bool AllowCopyToClipboard { get; set; }

        /// <summary>
        /// Gets or sets the mode of how to handle paste of clipboard data.
        /// </summary>
        [EditorBrowsable]
        public PasteFromClipboardMode PasteFromClipboardMode { get; set; } = PasteFromClipboardMode.PasteFullToken;

        /// <summary>
        /// The length of the token, defaults to 6
        /// </summary>
        [EditorBrowsable]
        public int TokenLength
        {
            get => _tokenLength;
            set
            {
                if (_tokenLength == value)
                {
                    return;
                }

                _tokenLength = value;
                _digits = new byte[_tokenLength];
                this.Invalidate();
            }
        }

        /// <summary>
        /// The corner radius of a digit box.
        /// </summary>
        [EditorBrowsable]
        public int CornerRadius { get; set; } = 5;

        /// <summary>
        /// The color of the border of a digit box.
        /// </summary>
        [EditorBrowsable]
        public Color BorderColor { get; set; } = Color.Gray;

        /// <summary>
        /// The size of each individual digit box.
        /// </summary>
        [EditorBrowsable]
        public Size BoxSize { get; set; } = new Size(25, 25);

        /// <summary>
        /// The gap between two digit boxes.
        /// </summary>
        [EditorBrowsable]
        public int BoxGap { get; set; } = 5;

        /// <summary>
        /// The resulting password token as entered by the user.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(true)]
        public PasswordToken PasswordToken
        {
            get
            {
                var result = new PasswordToken();
                for (int i = 0; i < _count; i++)
                {
                    result.AppendDigit(_digits[i]);
                }

                return result;
            }
        }

        /// <inheritdoc />
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int width = this.Width - this.Margin.Right - this.Margin.Left - 1;
            int height = this.Height - this.Margin.Bottom - this.Margin.Top - 1;
            _paintTransparentBackground(e, new Rectangle(0, 0, width, height));

            int reqWidth = _tokenLength * this.BoxSize.Width + (_tokenLength - 1) * this.BoxGap;
            int posX = (this.Width - reqWidth) / 2;
            int posY = (this.Height - this.BoxSize.Height) / 2;

            using (SolidBrush background = new SolidBrush(this.BackColor))
            using (Pen border = new Pen(this.BorderColor))
            using (Pen highlight = new Pen(SystemColors.Highlight))
            {
                for (int i = 0; i < _tokenLength; i++)
                {
                    g.FillRoundedRectangle(background, posX, posY, this.BoxSize.Width, this.BoxSize.Height,
                        this.CornerRadius);
                    Pen pen = this.Focused && i == _caretPos ? highlight : border;
                    g.DrawRoundedRectangle(pen, posX, posY, this.BoxSize.Width, this.BoxSize.Height, this.CornerRadius);

                    posX += this.BoxSize.Width + this.BoxGap;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            {
                for (int i = 0; i < _count; i++)
                {
                    string text = _digits[i].ToString();
                    var textSize = g.MeasureString(text, this.Font);
                    Point pos = this.BoxPos(i);
                    pos.X += (int)Math.Round((this.BoxSize.Width - textSize.Width) / 2);
                    pos.Y += (int)Math.Round((this.BoxSize.Height - textSize.Height) / 2);

                    g.DrawString(text, this.Font, textBrush, pos);
                }
            }
        }

        private Point BoxPos(int index)
        {
            if (index > _tokenLength - 1) index = _tokenLength - 1;
            if (index < 0) index = 0;

            int reqWidth = _tokenLength * this.BoxSize.Width + (_tokenLength - 1) * this.BoxGap;
            int posX = (this.Width - reqWidth) / 2;
            int posY = (this.Height - this.BoxSize.Height) / 2;

            if (index > 0)
            {
                posX += index * (this.BoxSize.Width + this.BoxGap);
            }

            return new Point(posX, posY);
        }

        /// <inheritdoc />
        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(6 * this.BoxSize.Width + 5 * this.BoxGap + 1, this.BoxSize.Height + 1);
        }

        private void InsertDigit(byte digit)
        {
            if (_count >= _tokenLength)
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
            if (_caretPos < _tokenLength - 1) _caretPos++;
            _count++;
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
        }

        /// <inheritdoc />
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && _caretPos < _tokenLength)
            {
                this.InsertDigit(Convert.ToByte(e.KeyChar.ToString()));
                this.MoveCaret();
                this.Invalidate();
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.AllowCopyToClipboard
                && e.KeyData == (Keys.C | Keys.Control))
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < _count; i++)
                {
                    stringBuilder.Append(_digits[i]);
                }

                StaClipboard.SetText(stringBuilder.ToString());
            }

            if (this.PasteFromClipboardMode != PasteFromClipboardMode.PasteNotAllowed
                && e.KeyData == (Keys.V | Keys.Control))
            {
                string text = StaClipboard.GetText();

                if (this.PasteFromClipboardMode == PasteFromClipboardMode.PasteFullToken
                    && text.All(char.IsDigit)
                    && text.Length == _tokenLength)
                {
                    // reset current input to copy full token text
                    _count = 0;
                    _caretPos = 0;
                    this.InsertDigitsFromText(text);
                }

                if (this.PasteFromClipboardMode == PasteFromClipboardMode.PasteAnyDigit)
                {
                    // paste as much digits as possible
                    this.InsertDigitsFromText(text);
                }

                this.MoveCaret();
                this.Invalidate();
            }

            base.OnKeyDown(e);
        }

        private void InsertDigitsFromText(string text)
        {
            int pasteCount = Math.Min(text.Length, _tokenLength - _count);
            int index = 0;
            while (pasteCount > 0 && index < text.Length)
            {
                if (char.IsDigit(text[index]))
                {
                    this.InsertDigit(Convert.ToByte(text.Substring(index, 1)));
                    pasteCount--;
                }

                index++;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Delete:
                    this.RemoveDigit();
                    this.Invalidate();
                    return true;
                case Keys.Back:
                    if (_caretPos > 0)
                    {
                        _caretPos--;
                        this.RemoveDigit();
                        this.MoveCaret();
                        this.Invalidate();
                        return true;
                    }

                    SystemSounds.Beep.Play();
                    return true;
                case Keys.Left:
                    if (_caretPos > 0)
                    {
                        _caretPos--;
                        this.MoveCaret();
                        this.Invalidate();
                    }

                    return true;
                case Keys.Right:
                    if (_caretPos < _count && _caretPos < _tokenLength - 1)
                    {
                        _caretPos++;
                        this.MoveCaret();
                        this.Invalidate();
                    }

                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveCaret()
        {
            var pos = this.BoxPos(_caretPos);
            SetCaretPos(pos.X + 3, pos.Y + this.BoxSize.Height - 3);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            CreateCaret(this.Handle, IntPtr.Zero, this.BoxSize.Width - 6, 1);
            this.MoveCaret();
            ShowCaret(this.Handle);
            this.Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            DestroyCaret();
            this.Invalidate();
            base.OnLostFocus(e);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBmp, int w, int h);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyCaret();
    }
}