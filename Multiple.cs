using System.Windows.Forms;

namespace Magic.SelectionButtonNET
{
    public class Multiple
    {

        // EVENT

        public event Action<SelectedMenuItemEventArgs>? SelectedMenuItemEvent;

        public class SelectedMenuItemEventArgs
        {

            public List<int> SelectedItemIds { get; set; }

            public SelectedMenuItemEventArgs(List<int> selectedItemIds)
            {

                SelectedItemIds = selectedItemIds;

            } // end of constructor method

        } // end of method

        // INPUT

        public ToolStripDropDownButton? ToolStripDropDownButton { get; set; }
        public Dictionary<int, string>? ItemData { get; set; } // data yang akan menjadi selection items
        public bool UseAll { get; set; } = true;

        // ID items yang terpilih. Initialize dari input, nantinya bisa berubah dan menjadi output
        public List<int> SelectedItemIds { get; set; } = new List<int>();

        // INTERNAL

        private const int AllValue = -1;

        public Multiple()
        {

        } // end of method

        public void InitializeToolStripMenu()
        {

            ToolStripDropDownButton!.DropDownItems.Clear(); // bersihkan semua selection item

            // CREATE MENU ITEMS

            bool allItemMenuChecked = false;

            if (UseAll)
            {
                string allItemText = "All";
                int allItemValue = AllValue;

                ToolStripMenuItem allItemMenu = new ToolStripMenuItem(allItemText);
                allItemMenu.Tag = allItemValue;
                allItemMenu.CheckOnClick = true;
                allItemMenu.Click += AllItemMenu_Click;

                if (SelectedItemIds.Contains(allItemValue))
                {
                    allItemMenu.Checked = true;
                    allItemMenuChecked = true;
                }

                ToolStripDropDownButton.DropDownItems.Add(allItemMenu);
            }

            if (ItemData!.Count > 0)
            {
                if (UseAll)
                {
                    ToolStripDropDownButton.DropDownItems.Add(new ToolStripSeparator());
                }

                foreach (KeyValuePair<int, string> item in ItemData)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(item.Value);
                    menuItem.Tag = item.Key;
                    menuItem.CheckOnClick = true;
                    menuItem.Click += MenuItem_Click;

                    if (SelectedItemIds.Contains(item.Key) || allItemMenuChecked)
                    {
                        menuItem.Checked = true;
                    }

                    ToolStripDropDownButton.DropDownItems.Add(menuItem);
                }
            }

        } // end of method

        private void AllItemMenu_Click(object? sender, EventArgs e)
        {

            ToolStripMenuItem allMenuItem = (sender as ToolStripMenuItem)!;

            List<int> selectedItemIds = new List<int>();

            foreach (ToolStripMenuItem item in ToolStripDropDownButton!.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    // Atur status centang item ke true
                    menuItem.Checked = allMenuItem.Checked;
                }
            }

            if (allMenuItem.Checked)
            {
                selectedItemIds.Add(AllValue);
            }

            SelectedItemIds = selectedItemIds;
            SelectedMenuItemEvent?.Invoke(new SelectedMenuItemEventArgs(SelectedItemIds));

        } // end of method

        private void MenuItem_Click(object? sender, EventArgs e)
        {

            ToolStripMenuItem menuItem = (sender as ToolStripMenuItem)!;

            List<int> selectedItemIds = new List<int>();
            ToolStripMenuItem allItemMenu = new ToolStripMenuItem();

            bool isAllMenuItemChecked = true;

            // iterasi semua menu item untuk cek kondisi all-checked atau tidak, juga untuk laporan output ke event
            foreach (ToolStripMenuItem item in ToolStripDropDownButton!.DropDownItems)
            {
                if (item is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Tag != null && (int)toolStripMenuItem.Tag == AllValue)
                {
                    // Ini adalah allItemMenu, catat & arahkan objeknya dulu
                    allItemMenu = toolStripMenuItem;
                    continue;
                }
                else if (item is ToolStripMenuItem toolStripMenuItem2 && toolStripMenuItem2.Tag != null && (int)toolStripMenuItem2.Tag != AllValue)
                {
                    // Ini adalah menu item lainnya, bukan allItemMenu, bukan ToolStripSeparator
                    if (toolStripMenuItem2.Checked)
                    {
                        // add dulu saja ke data terpilih
                        selectedItemIds.Add((int)toolStripMenuItem2.Tag);
                    }
                    else
                    {
                        isAllMenuItemChecked = false;
                    }
                }
                else
                {
                    // ini adalah separator
                    continue;
                }
            }

            if (isAllMenuItemChecked && UseAll)
            {
                allItemMenu.Checked = true;

                // yang sudah diadd, bersihkan semua karena mau dikasih AllValue (-1) (all selected) saja 
                selectedItemIds.Clear();
                selectedItemIds.Add(AllValue);
            }
            else if(UseAll)
            {
                allItemMenu.Checked = false;
            }

            SelectedItemIds = selectedItemIds;
            SelectedMenuItemEvent?.Invoke(new SelectedMenuItemEventArgs(SelectedItemIds));

        } // end of method

    } // end of class
} // end of namespace
