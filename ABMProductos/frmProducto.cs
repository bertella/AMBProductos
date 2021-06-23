using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ABMProductos
{
    public partial class frmProducto : Form
    {
        //nuevo = false: ejecuta insert en el boton grabar/ nuevo = true: ejecuta el update en el boton grabar
        bool nuevo = true;

        //CONEXION A SQL
        SqlConnection conexion = new SqlConnection(@"Data Source=DESKTOP-VODM8NI\SQLEXPRESS;Initial Catalog=Informatica;Integrated Security=True");
        SqlCommand comando = new SqlCommand();
        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            habilitar(false);
            cargarCombo(cboMarca, "Marcas");//Cargo el combo con e(nombre del combo,"Nombre de la tabla")
            cargarLista(lstProducto, "Productos");//Cargo la listBox con (nombre de la lisBox,"Nombre de la tabla")

        }

        //----------------------------------------------------------------METODOS--------------------------------------------------
        //muestra la tabla actualizada
        private void actualizarSql(string consultaSql)
        {
            conexion.Open();
            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;
            comando.CommandText = consultaSql;
            comando.ExecuteNonQuery();
            conexion.Close();

        }
        //carga la listbox
        private void cargarLista(ListBox lista, string nombreTabla)
        {
            DataTable tabla = consultarSql(consultaSql:"select str(codigo) ,'    '+str(codigo)+'    ' +detalle + '    '+' $'+str(precio) as INFO  from " + nombreTabla );
            lista.DataSource = tabla;
            //valor del item de la tabla
            lista.ValueMember = tabla.Columns[0].ColumnName;
            // nombre de columnas que quiero mostrar
            lista.DisplayMember = tabla.Columns[1].ColumnName;
        }
        //carga el comgobox
        private void cargarCombo(ComboBox combo,string nombreTabla)//metodo cargar combo box
        {
            DataTable tabla = consultarSql("select * from " + nombreTabla + " order by 2");
            combo.DataSource = tabla;
            combo.ValueMember = tabla.Columns[0].ColumnName;
            combo.DisplayMember = tabla.Columns[1].ColumnName;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;//me desailita el escribir en el cmbobox
        }
        //recibe un string que es la sentencia SQL y me devuelve una tabla cargada con esa sentencia
        private DataTable consultarSql(string consultaSql)//me devuelve la consulta como un datatable
        {
            conexion.Open();

            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;//dice q es tipo texto 
            comando.CommandText = consultaSql;

            DataTable tabla = new DataTable();
            tabla.Load(comando.ExecuteReader());

            conexion.Close();
            return tabla;

        }
        //--------------------------------------METODOS SECUNDARIOS-----------------------------------

        //habilita o inhabilita los campos
        private void habilitar(bool x)
        {
            txtCodigo.Enabled = x;
            txtDetalle.Enabled = x;
            cboMarca.Enabled = x;
            rbtNoteBook.Enabled = x;
            rbtNetBook.Enabled = x;
            txtPrecio.Enabled = x;
            dtpFecha.Enabled = x;
            lstProducto.Enabled = !x;
            btnGrabar.Enabled = x;
            btnCancelar.Enabled = x;
            btnBorrar.Enabled = !x;
            lstProducto.Enabled = !x;
        }
        private void limpiar()
        {
            txtCodigo.Text = "";
            txtDetalle.Text = "";
            rbtNetBook.Checked = false;
            rbtNoteBook.Checked = false;
            txtPrecio.Text = "";
        }
        private bool validarCampos()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                MessageBox.Show("Debe ingresar un codigo.");
                txtCodigo.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtDetalle.Text))
            {
                MessageBox.Show("Debe ingresar los detalles.");
                txtDetalle.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtPrecio.Text))
            {
                MessageBox.Show("Debe ingresar el precio.");
                txtPrecio.Focus();
                return false;
            }
            else

            return true;
        }
        //------------------------------------------------------------FIN METODOS ----------------------------------------------
        private void cboMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {//si los campos estan correctamente cargados validar campos devuelve true y crea el objeto producto
            if (validarCampos())
            {
                //crea el objeto producto
                Producto p = new Producto();
                p.pCodigo = Convert.ToInt32(txtCodigo.Text);
                p.pDetalle = txtDetalle.Text;
                p.pMarca = Convert.ToInt32(cboMarca.SelectedValue);
                p.pPrecio = Convert.ToDouble(txtPrecio.Text);
                p.pFecha = dtpFecha.Value;
                if (rbtNoteBook.Checked)
                {
                    p.pTipo = 1;
                }
                else
                    p.pTipo = 2;
                //si nuevo == true ejecuta el insert, si es false ejecuta el update (el boton nuevo setea nuevo==true y el boton editar setea nuevo==false)
                if (nuevo)
                {
                    string insertSql = "INSERT INTO productos VALUES (" + p.pCodigo + ",'" + p.pDetalle + "'," + p.pTipo + "," + p.pMarca + "," + p.pPrecio + ", '" + p.pFecha.ToString("yyyy/MM/dd") + "')";
                    actualizarSql(insertSql);
                    cargarLista(lstProducto, "Productos");
                }
                else
                {
                    string updateSql = "UPDATE productos SET detalle= '" + p.pDetalle + "'," + "tipo=" + p.pTipo + "," + "marca=" + p.pMarca + "," + "precio=" + p.pPrecio + "," + "fecha='" + p.pFecha.ToString("yyyy/MM/dd") + "'" + "WHERE codigo=" + p.pCodigo;
                    actualizarSql(updateSql);
                    cargarLista(lstProducto, "Productos");
                }
                
                habilitar(false);
                limpiar();
                nuevo = true;
            }          
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            nuevo = true;
            habilitar(true);
            txtCodigo.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //mensaje que le pregunta al usuario si desea salir
            if (MessageBox.Show("Seguro de abandonar la aplicación ?",
            "SALIR", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {

            habilitar(false);
            limpiar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {//setea nuevo == false para que al apretar el boton grabar ejecute el update
            nuevo = false;
            habilitar(true);
            txtCodigo.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            //ejecuta la sentencia delete from
            string deleteSql = "DELETE FROM productos WHERE codigo = " + lstProducto.SelectedValue.ToString();
            actualizarSql(deleteSql);
            cargarLista(lstProducto, "Productos");
        }

        private void lstProducto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
