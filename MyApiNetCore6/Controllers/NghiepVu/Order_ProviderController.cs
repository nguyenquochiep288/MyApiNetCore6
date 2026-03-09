<?xml version="1.0" encoding="utf-8"?>
<doc xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <assembly>
    <name>DevExpress.Map.v18.2.Core</name>
  </assembly>
  <members>
    <member name="N:DevExpress.Map">
      <summary>
        <para>Contains common utility classes used by the Map controls from DevExpress.</para>
      </summary>
    </member>
    <member name="T:DevExpress.Map.AttributeDisplayValueEditEventArgs">
      <summary>
        <para>Provides data for the <see cref="E:DevExpress.XtraMap.MapItemsLayerBase.AttributeDisplayValueEdit"/> and <see cref="E:DevExpress.Xpf.Map.VectorLayerBase.AttributeDisplayValueEdit"/> events.</para>
      </summary>
    </member>
    <member name="M:DevExpress.Map.AttributeDisplayValueEditEventArgs.#ctor(System.String,System.Object,System.String,System.String)">
      <summary>
        <para>Initializes a new instance of the <see cref="T:DevExpress.Map.AttributeDisplayValueEditEventArgs"/> class with the specified values of event arguments.</para>
      </summary>
      <param name="name">A <see cref="T:System.String"/> object representing the name of an attribute.</param>
      <param name="value">An <see cref="T:System.Object"/> storing an attribute value.</param>
      <param name="displayValue">A <see cref="T:System.String"/> object that is the string representation of the attribute value.</param>
      <param name="patternFragment">A <see cref="T:System.String"/> object representing a fragment of a text pattern, which will be replaced with the display value.</param>
    </member>
    <member name="P:DevExpress.Map.AttributeDisplayValueEditEventArgs.DisplayValue">
      <summary>
        <para>Gets or sets an attribute display value.</para>
      </summary>
      <value>A <see cref="T:System.String"/> object that is the string representation of the attribute value.</value>
    </member>
    <member name="P:DevExpress.Map.AttributeDisplayValueEditEventArgs.Name">
      <summary>
        <para>Returns the name of an attribute.</para>
      </summary>
      <value>A <see cref="T:System.String"/> object representing the name of attribute whose display value should be customized.</value>
    </member>
    <member name="P:DevExpress.Map.AttributeDisplayValueEditEventArgs.PatternFragment">
      <summary>
        <para>Returns the pattern fragment which will be replaced with the <see cref="P:DevExpress.Map.AttributeDisplayValueEditEventArgs.DisplayValue"/>.</para>
      </summary>
      <value>A <see cref="T:System.String"/> object representing the fragment pattern.</value>
    </member>
    <member name="P:DevExpress.Map.AttributeDisplayValueEditEventArgs.Value">
      <summary>
        <para>Returns an attribute value.</para>
      </summary>
      <value>A <see cref="T:System.Object"/> storing an attribute value.</value>
    </member>
    <member name="T:DevExpress.Map.AttributeDisplayValueEditEventHandler">
      <summary>
        <para>A method that will handle the <see cref="E:DevExpress.XtraMap.MapItemsLayerBase.AttributeDisplayValueEdit"/> and <see cref="E:DevExpress.Xpf.Map.VectorLayerBase.AttributeDisplayValueEdit"/> events.</para>
      </summary>
      <param name="sender">The event source. This parameter identifies the <see cref="T:DevExpress.XtraMap.MapItemsLayerBase"/> (WinForms) or <see cref="T:DevExpress.Xpf.Map.VectorLayerBase"/> (WPF) which raised the event.</param>
      <param name="e"></param>
    </member>
    <member name="T:DevExpress.Map.CoordPoint">
      <summary>
        <para>The base class for all map coordinate points.</para>
      </summary>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.Equals(System.Object)">
      <summary>
        <para>Determines whether the two specified <see cref="T:DevExpress.Map.CoordPoint"/> objects are equal.</para>
      </summary>
      <param name="o">The object to compare with the current object.</param>
      <returns>true if specified objects are equal; otherwise false.</returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.GetHashCode">
      <summary>
        <para>Gets the hash code (a number) that corresponds to the value of the current <see cref="T:DevExpress.Map.CoordPoint"/> object.</para>
      </summary>
      <returns>An integer value that is the hash code for the current object.</returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.GetX">
      <summary>
        <para>Returns the value of the X coordinate.</para>
      </summary>
      <returns>A <see cref="T:System.Double"/> value.</returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.GetY">
      <summary>
        <para>Returns the value of the Y coordinate.</para>
      </summary>
      <returns>A <see cref="T:System.Double"/> value.</returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.Offset(System.Double,System.Double)">
      <summary>
        <para>Initializes an instance of a <see cref="T:DevExpress.Map.CoordPoint"/> class descendant object offset by specified values.</para>
      </summary>
      <param name="offsetX">A <see cref="T:System.Double"/> value specifying an X coordinate offset.</param>
      <param name="offsetY">A <see cref="T:System.Double"/> value specifying an Y coordinate offset.</param>
      <returns>A <see cref="T:DevExpress.Map.CoordPoint"/> class descendant object.</returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.op_Equality(DevExpress.Map.CoordPoint,DevExpress.Map.CoordPoint)">
      <summary />
      <param name="point1"></param>
      <param name="point2"></param>
      <returns></returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.op_Inequality(DevExpress.Map.CoordPoint,DevExpress.Map.CoordPoint)">
      <summary />
      <param name="point1"></param>
      <param name="point2"></param>
      <returns></returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.ToString">
      <summary>
        <para>Returns the textual representation of the <see cref="T:DevExpress.Map.CoordPoint"/>.</para>
      </summary>
      <returns>A <see cref="T:System.String"/> value, which is the textual representation of the <see cref="T:DevExpress.Map.CoordPoint"/>.</returns>
    </member>
    <member name="M:DevExpress.Map.CoordPoint.ToString(System.IFormatProvider)">
      <summary>
        <para>Returns the textual representation of the <see cref="T:DevExpress.Map.CoordPoint"/>.</para>
      </summary>
      <param name="provider">An object implementing the <see cref="T:System.IFormatProvider"/> interface.</param>
      <returns>A <see cref="T:System.String"/> value, which is the textual representation of the <see cref="T:DevExpress.Map.CoordPoint"/>.</returns>
    </member>
    <member name="T:DevExpress.Map.ISupportCoordLocation">
      <summary>
        <para>The interface that should be provided by map vector items whose location can be determined.</para>
      </summary>
    </member>
    <member name="P:DevExpress.Map.ISupportCoordLocation.Location">
      <summary>
        <para>Gets or sets the location of map items implementing this interface.</para>
      </summary>
      <value>A <see cref="T:DevExpress.Map.CoordPoint"/> class descendant object.</value>
    </member>
    <member name="T:DevExpress.Map.ISupportCoordPoints">
      <summary>
        <para>The interface that should be implemented by map vector items specified using an array of points.</para>
      </summary>
    </member>
    <member name="P:DevExpress.Map.ISupportCoordPoints.Points">
      <summary>
        <para>Gets or sets the list of points determining the shape of map items implementing this interface.</para>
      </summary>
      <value>A list of <see cref="T:DevExpress.Map.CoordPoint"/> class descendant objects.</value>
    </member>
    <member name="T:DevExpress.Map.RangeDistributionBase">
      <summary>
        <para>The base for classes that define distribution of color ranges in a colorizer.</para>
      </summary>
    </member>
    <member name="M:DevExpress.Map.RangeDistributionBase.ConvertRangeValue(System.Double,System.Double,System.Double)">
      <summary>
        <para>Converts the specified range value.</para>
      </summary>
      <param name="min">A <see cref="T:System.Double"/> object that is the minimum possible value.</param>
      <param name="max">A <see cref="T:System.Double"/> object that is the maximum possible value.</param>
      <param name="value">A value to be converted.</param>
      <returns>A <see cref="T:System.Double"/> object that is the result of the conversion.</returns>
    </member>
  </members>
</doc>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              <?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name="resmimetype">text/microsoft-resx</resheader>
    <resheader name="version">2.0</resheader>
    <resheader name="reader">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name="writer">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name="Name1"><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name="Color1" type="System.Drawing.Color, System.Drawing">Blue</data>
    <data name="Bitmap1" mimetype="application/x-microsoft.net.object.binary.base64">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name="Icon1" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of "resheader" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <metadata name="dsHoaDonBanHang.TrayLocation" type="System.Drawing.Point, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a">
    <value>17, 17</value>
  </metadata>
</root>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        Message = "Success",
                    Data = ct_PhieuDatHangNCC
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        // DELETE: api/Input/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
        {
            try
            {
                var Input = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                if (Input.ISHOANTAT)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Phiếu " + Input.MAPHIEU + " đã hoàn thành! Vui lòng kiểm tra lại!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHangNCC_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuDatHangNCC_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                            }
                            else
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
                                    Data = ""
                                });
                            }
                            _context.ct_PhieuDatHangNCC_ChiTiet!.Remove(itm);
                        }
                    }

                    _context.ct_PhieuDatHangNCC!.Remove(Input);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
               
                transaction.Commit();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private bool InputExistsID(string LOC_ID, string ID)
        {
            return _context.ct_PhieuDatHangNCC!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}