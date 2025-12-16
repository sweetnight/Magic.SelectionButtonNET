using System.Drawing;
using System.Windows.Forms;

namespace Magic.SelectionButtonNET
{
    /// <summary>
    /// Class ini hanya menambahkan item di dropdown menu.
    /// Untuk mengaktifkan atau menon-aktifkan dropdown menu bukan jobdesk kelas ini.
    /// </summary>
    public class Single
    {
        public event Action<MenuItemSelectedEventArgs>? MenuItemSelectedEvent;

        public class MenuItemSelectedEventArgs
        {

            public ToolStripMenuItem SelectedMenuItem { get; set; }
            public int SelectedItemId { get; set; }

            public MenuItemSelectedEventArgs(ToolStripMenuItem selectedMenuItem, int selectedItemId)
            {

                this.SelectedMenuItem = selectedMenuItem;
                SelectedItemId = selectedItemId;
            } // end of constructor method

        } // end of method

        public ToolStripDropDownButton? ToolStripDropDownButton { get; set; }
        public Dictionary<int, string>? ItemData { get; set; }
        public int SelectedItemId { get; set; }

        private ToolStripMenuItem? SelectedMenuItem { get; set; }

        public Single()
        {

        } // end of method

        public void InitializeToolStripMenu()
        {

            ToolStripDropDownButton!.DropDownItems.Clear(); // bersihkan anak itemnya

            // BUAT MASING2 PILIHAN DI UI SELECTION & LISTENING JIKA PILIHAN DIKLIK (EVENT)
            if (ItemData!.Count > 0)
            {
                // SelectedItemId tidak ada di Dictionary ItemData, jadi selectednya set default
                if (!ItemData.ContainsKey(SelectedItemId))
                {
                    SelectedItemId = ItemData.Keys.First();
                }

                foreach (KeyValuePair<int, string> item in ItemData!)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(item.Value);
                    menuItem.Tag = item.Key;
                    menuItem.CheckOnClick = false;
                    menuItem.Click += MenuItem_Click;

                    if (item.Key == SelectedItemId)
                    {
                        SelectedMenuItem = menuItem;
                        SetMenuItemUI(menuItem, true);
                    }

                    ToolStripDropDownButton.DropDownItems.Add(menuItem);
                }
            }

            // KIRIM DATA SELECTED VIA EVENT
            InvokeEvent();

        } // end of method

        private void MenuItem_Click(object? sender, EventArgs e)
        {

            ToolStripMenuItem? clickedMenuItem = sender as ToolStripMenuItem;

            // UNSELECT SEBELUMNYA (UI)
            if (SelectedMenuItem != null)
            {
                SetMenuItemUI(SelectedMenuItem, false);
            }

            // BUAT SELECTION YANG BARU (UI)
            SetMenuItemUI(clickedMenuItem!, true);
            SelectedMenuItem = clickedMenuItem;

            // SAVE SELECTION YANG BARU KE DATABASE
            SelectedItemId = (int)SelectedMenuItem!.Tag!;

            // KIRIM RESULT VIA EVENT
            InvokeEvent();

        } // end of method

        private void SetMenuItemUI(ToolStripMenuItem menuItem, bool isSelected)
        {
            if (isSelected)
            {
                menuItem.Image = CreateFilledCircleIcon(Color.Green);
                menuItem.Checked = false; // Set Checked to false to remove default check mark
            }
            else
            {
                menuItem.Image = null;
            }
        } // end of method

        private Image CreateFilledCircleIcon(Color color)
        {

            Bitmap bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 4, 4, 8, 8); // Diameter 8
                }
            }

            return bitmap;

        } // end of method

        public static string TrimTextToFit(ToolStripDropDownButton button, string text)
        {

            string ellipsis = "...";

            using (Graphics g = button.Owner!.CreateGraphics())
            {
                // Mengukur lebar maksimal teks yang bisa ditampilkan
                int maxWidth = button.Width - 30;

                // Mengukur lebar teks asli
                int textWidth = (int)g.MeasureString(text, button.Font).Width;

                // Jika lebar teks tidak melebihi lebar maksimal, kembalikan teks asli
                if (textWidth <= maxWidth) return text;

                // Mengukur lebar ellipsis
                int ellipsisWidth = (int)g.MeasureString(ellipsis, button.Font).Width;

                string subText = string.Empty;

                // Potong teks dan tambahkan ellipsis
                for (int i = text.Length; i > 0; i--)
                {
                    subText = text.Substring(0, i);
                    int subTextWidth = (int)g.MeasureString(subText, button.Font).Width;

                    if (subTextWidth + ellipsisWidth <= maxWidth) break;
                }

                return subText.Trim() + ellipsis;
            }

        } // end of method

        private void InvokeEvent()
        {

            ToolStripDropDownButton!.Text = TrimTextToFit(ToolStripDropDownButton, SelectedMenuItem!.Text!);
            MenuItemSelectedEvent?.Invoke(new MenuItemSelectedEventArgs(SelectedMenuItem!, SelectedItemId));

        } // end of method

    } // end of class
} // end of namespace
