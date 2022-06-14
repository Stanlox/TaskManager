using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManager
{
    class ListViewItemComparator : IComparer
    {
        private int columnIndex;
        private SortOrder sortDirection;

        public int ColumnIndex
        {
            get
            {
                return columnIndex;
            }
            set
            {
                columnIndex = value;
            }
        }

        public SortOrder SortDirection
        {
            get
            {
                return sortDirection;
            }
            set
            {
                sortDirection = value;
            }
        }

        public ListViewItemComparator()
        {
            sortDirection = SortOrder.None;
        }

        public int Compare(object x, object y)
        {
           ListViewItem listViewItemX = x as ListViewItem;
           ListViewItem listViewItemY = y as ListViewItem;

           int result;

           switch (columnIndex)
            {
                case 0:
                    result = string.Compare(listViewItemX.SubItems[columnIndex].Text,
                        listViewItemY.SubItems[columnIndex].Text, false);
                    break;
                case 1:
                    double valueX = double.Parse(listViewItemX.SubItems[columnIndex].Text);
                    double valueY = double.Parse(listViewItemY.SubItems[columnIndex].Text);
                    result = valueX.CompareTo(valueY);
                    break;
                default:
                    result = string.Compare(listViewItemX.SubItems[columnIndex].Text,
                        listViewItemY.SubItems[columnIndex].Text, false);
                    break;
            }

            if (sortDirection == SortOrder.Descending)
            {
                return -result;
            }
            else
            {
                return result;
            }
        }
    }
}
