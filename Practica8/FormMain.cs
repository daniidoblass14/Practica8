/*
* PR�CTICA.............: Pr�ctica 8
* NOMBRE y APELLIDOS...: Daniel Jes�s Doblas Florido
* CURSO y GRUPO........: 2� Desarrollo de Interfaces
* T�TULO de la PR�CTICA: Uso del IDE V.Studio
* FECHA de ENTREGA.....: 8 de febrero de 2023
*/

using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Practica8
{
    partial class FormMain : Form
    {
        public List<Grupo> grupos;

        public static Grupo grupoSel;
        public FormMain()
        {
            InitializeComponent();

            grupos = new List<Grupo>();
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.CenterToScreen();


            crearColumnasGrupo();
        }

        public void crearColumnasGrupo()
        {
            //Defino el objeto del que quiero crear las columnas
            Type tipo = typeof(Grupo);

            // Obtener las propiedades del tipo
            PropertyInfo[] properties = tipo.GetProperties();

            DataGridViewColumn column = new DataGridViewColumn();
            column.Name = properties[0].Name;
            column.HeaderText = properties[0].Name;
            column.ValueType = tipo;
            column.CellTemplate = new DataGridViewTextBoxCell();

            dtgvGrupos.Columns.Add(column);

            // Recorrer las propiedades
            for (int i = 1; i < properties.Length - 1; i++)
            {
                column = new DataGridViewColumn();
                column.Name = properties[i].Name;
                column.HeaderText = properties[i].Name;
                column.ValueType = properties[i].PropertyType;
                column.CellTemplate = new DataGridViewTextBoxCell();

                // Agregar la columna al DataGridView
                dtgvGrupos.Columns.Add(column);
            }
        }

        public void crearColumnasAlumno()
        {
            dtgvAlumnos.Columns.Clear();
            //Defino el objeto del que quiero crear las columnas
            Type tipo = typeof(Alumno);
            // Obtener las propiedades del tipo
            PropertyInfo[] properties = tipo.GetProperties();

            DataGridViewColumn column = new DataGridViewColumn();
            column.Name = properties[0].Name;
            column.HeaderText = properties[0].Name;
            column.ValueType = tipo;
            column.CellTemplate = new DataGridViewTextBoxCell();

            dtgvAlumnos.Columns.Add(column);

            // Recorrer las propiedades
            for (int i = 1; i < properties.Length; i++)
            {
                if (properties[i].PropertyType.Name.Contains("[]"))
                {

                    for (int j = 0; j < grupoSel.CodigosAsignaturas.Length; j++)
                    {
                        column = new DataGridViewColumn();
                        column.Name = grupoSel.CodigosAsignaturas[j];
                        column.HeaderText = grupoSel.CodigosAsignaturas[j];
                        column.ValueType = typeof(double);
                        column.CellTemplate = new DataGridViewTextBoxCell();

                        dtgvAlumnos.Columns.Add(column);
                    }
                }
                else
                {
                    column = new DataGridViewColumn();
                    column.Name = properties[i].Name;
                    column.HeaderText = properties[i].Name;
                    column.ValueType = properties[i].PropertyType;
                    column.CellTemplate = new DataGridViewTextBoxCell();

                    // Agregar la columna al DataGridView
                    dtgvAlumnos.Columns.Add(column);
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FormCrearGrupo formCrearGrupo = new FormCrearGrupo();

            if (formCrearGrupo.ShowDialog() == DialogResult.OK)
            {
                Grupo grupoAgregado = formCrearGrupo.Grupo;
                insertarRegistro(grupoAgregado);

                grupoSel = grupoAgregado;
                crearColumnasAlumno();
                actualizarAlumnos(grupoSel.Alumnos);
            }
        }

        private void btnBorrarGrupo_Click(object sender, EventArgs e)
        {
            try
            {
                if (grupoSel != null)
                {
                    if (MessageBox.Show("�Deseas borrar el grupo " + grupoSel.Nombre + "?", "Confirmaci�n", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        grupos.Remove(grupoSel);
                        MessageBox.Show("Se ha borrado correctamente el grupo " + grupoSel.Nombre, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        actualizarGrupos();
                    }
                    else
                    {
                        MessageBox.Show("No se ha borrado el grupo " + grupoSel.Nombre, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No hay ning�n grupo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("ERROR AL BORRAR, NO SE PUEDO REALIZAR LA ACCI�N" + grupoSel.Nombre, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Archivos GRU (*.gru)|*.gru";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            ofd.RestoreDirectory = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream ficheroLeer = new FileStream(ofd.FileName, FileMode.Open);

                BinaryFormatter formatter = new BinaryFormatter();

                Grupo grupoImportado = (Grupo)formatter.Deserialize(ficheroLeer);

                bool repetido = false;

                for (int i = 0; i < grupos.Count; i++)
                {
                    if (grupos[i].Nombre.Equals(grupoImportado.Nombre))
                    {
                        repetido = true;
                    }
                }

                if (repetido)
                {
                    MessageBox.Show("El grupo a importar ya existe", "Error al importar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    insertarRegistro(grupoImportado);
                    MessageBox.Show("Se ha importado correctamente el grupo", "Grupo creado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }

                ficheroLeer.Close();

                
            }
        }
        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (grupoSel == null)
            {
                MessageBox.Show("No hay ning�n grupo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "Archivos GRU (*.gru)|*.gru";
                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;
                sfd.FileName = grupoSel.Nombre;
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Serialize the object to the specified file
                    using (FileStream ficheroGuardar = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(ficheroGuardar, grupoSel);
                    }
                    MessageBox.Show("Se ha exportado correctamente el grupo " + grupoSel.Nombre, "Grupo importado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnAgregarAlumno_Click(object sender, EventArgs e)
        {

            if (FormMain.grupoSel != null)
            {
                FormDatosAlumno formCrearAlumno = new FormDatosAlumno();
                if (formCrearAlumno.ShowDialog() == DialogResult.OK)
                {
                    insertarRegistro(grupoSel.Alumnos[grupoSel.Alumnos.Count - 1]);
                }
            }
            else
            {
                MessageBox.Show("No hay un grupo seleccionado.");
                // No abre el formulario para agregar alumnos
            }

        }

        private void btnBorrarAlumno_Click(object sender, EventArgs e)
        {
            if (dtgvAlumnos.SelectedRows.Count > 0 && dtgvAlumnos.SelectedRows[0].Cells[1].Value != null && dtgvAlumnos.SelectedRows[0].Cells[1].Value is Alumno)
            {
                Alumno alumnoSel = (Alumno)dtgvAlumnos.SelectedRows[0].Cells[1].Value;

                if (dtgvAlumnos.SelectedRows.Count > 0 && dtgvAlumnos.SelectedRows[0].Cells[1].Value != null)
                {
                    //Alumno alumnoSel = (Alumno)dtgvAlumnos.SelectedRows[0].Cells[1].Value;
                    if (MessageBox.Show("�Deseas borrar al alumno " + alumnoSel.Nombre + " con matr�cula " + alumnoSel.Matricula + "?", "Confirmaci�n", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        grupoSel.borrarAlumno(alumnoSel.Matricula);
                        MessageBox.Show("Se ha borrado correctamente al alumno " + alumnoSel.Nombre + " con matr�cula " + alumnoSel.Matricula, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        actualizarAlumnos(grupoSel.Alumnos);
                    }
                    else
                    {
                        MessageBox.Show("No se ha borrado al alumno " + alumnoSel.Nombre + " con matr�cula " + alumnoSel.Matricula, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Debe seleccionar una fila con un alumno v�lido para eliminar.");
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una fila con un alumno v�lido para eliminar.");
            }
            

            

            
        }

        private void btnEditarAlumno_Click(object sender, EventArgs e)
        {
            if (dtgvAlumnos.SelectedRows.Count > 0 && dtgvAlumnos.SelectedRows[0].Cells[1].Value != null)
            {
                Alumno alumnoSel = (Alumno)dtgvAlumnos.SelectedRows[0].Cells[1].Value;

                FormDatosAlumno formCrearAlumno = new FormDatosAlumno(alumnoSel);
                formCrearAlumno.ShowDialog();
                actualizarAlumnos(grupoSel.Alumnos);
            }
            else
            {
                MessageBox.Show("Debe seleccionar un alumno.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnFiltrarAlumno_Click(object sender, EventArgs e)
        {
            if (btnFiltrarAlumnos.Text != "Quitar filtro")
            {
                List<Alumno> alumnosAprobados = new List<Alumno>();
                for (int i = 0; i < grupoSel.Alumnos.Count; i++)
                {
                    bool aprobado = true;
                    for (int j = 0; j < grupoSel.Alumnos[i].Notas.Length; j++)
                    {
                        if (grupoSel.Alumnos[i].Notas[j] < 5)
                        {
                            aprobado = false;
                        }
                    }
                    if (aprobado)
                    {
                        alumnosAprobados.Add(grupoSel.Alumnos[i]);
                    }
                }
                actualizarAlumnos(alumnosAprobados);
                btnFiltrarAlumnos.Text = "Quitar filtro";
            }
            else
            {
                actualizarAlumnos(grupoSel.Alumnos);
                btnFiltrarAlumnos.Text = "Filtrar aprobados";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void insertarRegistro(Grupo grupo)
        {
            // Obtener las propiedades del objeto
            PropertyInfo[] properties = grupo.GetType().GetProperties();

            // Crear una nueva fila
            int rowIndex = dtgvGrupos.Rows.Add();
            DataGridViewRow row = dtgvGrupos.Rows[rowIndex];

            row.Cells[0].Value = grupo;

            // Recorrer las propiedades
            for (int i = 1; i < properties.Length; i++)
            {
                if (properties[i].PropertyType.Name.Contains("[]"))
                {
                    string[] asignaturas = (string[])properties[i].GetValue(grupo);

                    row.Cells[i].Value = string.Join(",", asignaturas);
                }
                else if (!properties[i].PropertyType.Name.Contains("List"))
                {
                    row.Cells[i].Value = properties[i].GetValue(grupo);
                }
                dtgvGrupos.CurrentCell = row.Cells[0];
                btnAgregarAlumno.Enabled = true;
                btnExportar.Enabled = true;
                btnBorrarGrupo.Enabled = true;
                row.Selected = true;
            }

            grupos.Add(grupo);
        }

        public void insertarRegistro(Alumno alumno)
        {

            if (dtgvAlumnos.Columns.Count > 0)
            {
                // Agregar la fila
                int rowIndex = dtgvAlumnos.Rows.Add();
                DataGridViewRow row = dtgvAlumnos.Rows[rowIndex];

                // Resto del c�digo...
                // Obtener las propiedades del objeto
                PropertyInfo[] properties = alumno.GetType().GetProperties();

                int numCelda = 0;

                // Recorrer las propiedades
                for (int i = 0; i < properties.Length; i++)
                {
                    if (numCelda == 1)
                    {
                        row.Cells[numCelda].Value = alumno;
                        numCelda++;
                    }
                    else
                    {
                        if (properties[i].PropertyType.Name.Contains("[]"))
                        {
                            double[] notas = (double[])properties[i].GetValue(alumno);

                            for (int j = 0; j < notas.Length; j++)
                            {
                                row.Cells[numCelda].Value = notas[j];

                                numCelda++;
                            }
                        }
                        else
                        {
                            row.Cells[numCelda].Value = properties[i].GetValue(alumno);
                            numCelda++;
                        }
                    }
                }
                dtgvAlumnos.CurrentCell = row.Cells[0];
                btnBorrarAlumno.Enabled = true;
                btnEditarAlumno.Enabled = true;
                btnFiltrarAlumnos.Enabled = true;
                row.Selected = true;
            }
            else
            {
                MessageBox.Show("No hay registros.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           
        }

        private void actualizarAlumnos(List<Alumno> alumnos)
        {
            dtgvAlumnos.Rows.Clear();
            for (int i = 0; i < alumnos.Count; i++)
            {
                insertarRegistro(alumnos[i]);
            }
        }
        private void actualizarGrupos()
        {
            dtgvGrupos.Rows.Clear();
            for (int i = 0; i < grupos.Count; i++)
            {
                insertarRegistro(grupos[i]);
            }
            dtgvAlumnos.Rows.Clear();
            dtgvAlumnos.Columns.Clear();
        }
        private void dtgvGrupos_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow filaSel = dtgvGrupos.Rows[e.RowIndex];
            if (filaSel != null)
            {
                grupoSel = (Grupo)filaSel.Cells[0].Value;
                if (grupoSel != null)
                {
                    crearColumnasAlumno();
                    actualizarAlumnos(grupoSel.Alumnos);
                }
            }
        }

        private void dtgvAlumnos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dtgvAlumnos.Rows[e.RowIndex].Selected = true;
        }

        private void dtgvGrupos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dtgvGrupos.Rows[e.RowIndex].Selected = true;
        }

        private void acercadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Programa realizado por Daniel Jes�s Doblas Florido\n" +
                "Versi�n 1.0.0", "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}