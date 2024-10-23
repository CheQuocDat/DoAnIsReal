using QuanLyCuaHang.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace QuanLyCuaHang
{
    public partial class AddProductUC : UserControl
    {
        private int numOfProductList = 0;

        private Panel currPanel;

        ConveStoreDBContext db;

        List<ProductViewModel> productListOfUC;
        List<SupplierViewModel> supplierListOfUC;
        List<GatetoryViewModel> gatetoryListOfUC;

        public AddProductUC(ref Panel currPanel)
        {
            InitializeComponent();

            db = new ConveStoreDBContext();

            productListOfUC = ProductViewModel.GetListProduct(db);
            supplierListOfUC = SupplierViewModel.GetList(db);
            gatetoryListOfUC = GatetoryViewModel.GetListGatetory(db);

            numOfProductList = productListOfUC.Count;

            this.currPanel = currPanel;
        }

        private void AddProductUC_Load(object sender, EventArgs e)
        {
            SupplierViewModel.BindToComboBox(cmbSupplier, supplierListOfUC);
            GatetoryViewModel.BindToComboBox(cmbCategory, gatetoryListOfUC);
        }

        #region Button Methods
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            bool isEmptyInput = string.IsNullOrEmpty(txtProductName.Text)
                    || string.IsNullOrEmpty(txtPrice.Text)
                    || cmbSupplier.SelectedIndex == -1
                    || cmbCategory.SelectedIndex == -1;

            try
            {
                if (isEmptyInput)
                    throw new Exception("Bạn phải chọn đầy đủ các lựa chọn");

                // Chưa làm kiểu kiểm tra tên sản phẩm đã có trong danh mục chưa

                SANPHAM newProduct = FillInputToEntityProduct();
                db.SANPHAMs.Add(newProduct);
                db.SaveChanges();

                throw new Exception("Thêm sản phẩm thành công");
            }
            catch (Exception ex) 
            {
                ResetInput();
                MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnBackToProductGateFrm_Click(object sender, EventArgs e)
        {
            // Ẩn UserControl
            this.Visible = false;

            // Ẩn panel chứa UserControl
            if (currPanel != null)
            {
                currPanel.Visible = false; // Ẩn panel
            }
        }
        #endregion

        #region Other Methods
        // Phương thức đổ input vào thực thể sản phẩm mới
        private SANPHAM FillInputToEntityProduct()
        {
            SANPHAM result = new SANPHAM();

            result.MaSP = CreateNewProductID();
            result.TenSP = txtProductName.Text.Trim();
            result.DonGiA = double.Parse(txtPrice.Text.Trim());

            result.MaNCC = (cmbSupplier.SelectedItem as SupplierViewModel).MaNCC;

            result.MaTheLoai = (cmbCategory.SelectedItem as GatetoryViewModel).MaTheLoai;

            return result;
        }

        // Phương thức tạo mới 1 mã sản phẩm
        private string CreateNewProductID()
        {
            string result = string.Empty;
            bool isTwoDigitNumber = numOfProductList <= 98 && numOfProductList >= 9;
            bool isThreeDigitNumber = numOfProductList <= 998 && numOfProductList >= 99;
            
            numOfProductList += 2;

            if (isTwoDigitNumber)
                result = "SP0" + numOfProductList.ToString().Trim();
            else if (isThreeDigitNumber)
                result = "SP" + numOfProductList.ToString().Trim();

            return result;
        }
        // Sửa chỗ này
        // Phương thức reset lại các giá trị input
        private void ResetInput()
        {
            txtProductName.Text = string.Empty;
            txtPrice.Text = string.Empty;
            cmbSupplier.SelectedIndex = -1;
            cmbCategory.SelectedIndex = -1;
        }
        #endregion
    }
}
