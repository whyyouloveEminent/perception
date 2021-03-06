﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form2Document.aspx.cs" Inherits="CMS.Form2Document" %>

<!DOCTYPE html >
<html xmlns="http://www.w3.org/1999/xhtml">
<!--[if IE 8]> <html lang="en" class="ie8"> <![endif]-->
<!--[if IE 9]> <html lang="en" class="ie9"> <![endif]-->
<!--[if !IE]><!-->
<html lang="en">
<!--<![endif]-->
<!-- BEGIN HEAD -->
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>Eminent | New Flow</title>
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta content="" name="description" />
    <meta content="" name="author" />
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/css/metro.css" rel="stylesheet" />
    <link href="assets/bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="assets/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="assets/css/style.css" rel="stylesheet" />
    <link href="assets/css/style_responsive.css" rel="stylesheet" />
    <link href="assets/css/style_default.css" rel="stylesheet" id="style_color" />
    <link rel="stylesheet" type="text/css" href="assets/gritter/css/jquery.gritter.css" />
    <link rel="stylesheet" type="text/css" href="assets/uniform/css/uniform.default.css" />
    <link rel="stylesheet" type="text/css" href="assets/chosen-bootstrap/chosen/chosen.css" />
    <link rel="stylesheet" type="text/css" href="assets/bootstrap-wysihtml5/bootstrap-wysihtml5.css" />
    <link rel="stylesheet" type="text/css" href="assets/bootstrap-datepicker/css/datepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/bootstrap-timepicker/compiled/timepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/bootstrap-colorpicker/css/colorpicker.css" />
    <link rel="stylesheet" href="assets/bootstrap-toggle-buttons/static/stylesheets/bootstrap-toggle-buttons.css" />
    <link rel="stylesheet" href="assets/data-tables/DT_bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="assets/bootstrap-daterangepicker/daterangepicker.css" />
    <link rel="stylesheet" type="text/css" href="assets/uniform/css/uniform.default.css" />
    <!-- Form Builder Styles -->
    <link href="assets/flow-css/css" rel="stylesheet" type="text/css">
    <link href="assets/flow-css/css(1)" rel="stylesheet" type="text/css">
    <link rel="stylesheet" href="assets/flow-css/style.css" type="text/css" media="all">
    <link rel="stylesheet" href="assets/flow-css/form.css" type="text/css" media="all">
    <link rel="stylesheet" href="assets/flow-css/style1.css" type="text/css" media="all" id="csstheme">
    <link rel="stylesheet" href="assets/flow-css/tipsy.css" type="text/css" media="all">
    <link rel="stylesheet" href="assets/flow-css/enhanced.css" type="text/css" media="all">
   
    <link rel="shortcut icon" href="favicon.ico" />
</head>
<!-- END HEAD -->
<!-- BEGIN BODY -->
<body class="fixed-top">
    <theme:header ID="header" runat="server" />
    <!-- BEGIN CONTAINER -->
    <div class="page-container row-fluid">
        <theme:sidebar ID="siderbar" runat="server" />
        <!-- BEGIN PAGE -->
        <div class="page-content">
            <!-- BEGIN SAMPLE PORTLET CONFIGURATION MODAL flow-->
            <div id="portlet-config" class="modal hide">
                <div class="modal-header">
                    <button data-dismiss="modal" class="close" type="button">
                    </button>
                    <h3>
                        portlet Settings</h3>
                </div>
                <div class="modal-body">
                    <p>
                        Here will be a configuration form</p>
                </div>
            </div>
            <!-- END SAMPLE PORTLET CONFIGURATION MODAL flow-->
            <!-- BEGIN PAGE CONTAINER-->
            <div class="container-fluid">
                <!-- BEGIN PAGE HEADER-->
                <div class="row-fluid">
                    <div class="span12">
                        <theme:customizer ID="customizer" runat="server" />
                        <!-- BEGIN PAGE TITLE & BREADCRUMB-->
                        <h3 class="page-title">New Flow<small></small>
                        </h3>
                        <ul class="breadcrumb">
                            <li><i class="icon-home"></i><a href="index.html">Home</a> <i class="icon-angle-right">
                            </i></li>
                            <li><a href="#">New Stage</a> <i class="icon-angle-right">
                            </i></li>
                            
                        </ul>
                        <!-- END PAGE TITLE & BREADCRUMB-->
                    </div>
                </div>
                <!-- END PAGE HEADER-->
                <!-- BEGIN PAGE CONTENT-->
                <div class="row-fluid">
                    <div class="span12">
                        <div id="Div1">
                            <div id="Grid_Controls">

                                <div id="buttons-container1">    
                                    <a href="#" id="add_widget" class="btn green">Add widget</a> 
                                    <a href="#" id="preview" class="btn grey">Preview</a> 
                                    <a href="#" id="saveform" class="btn blue">Save</a>
                                </div>
                                <div class="">
									
							    </div>                            
                            </div>

                        </div>
                        <div style="margin-top:50px;" id="container">

                        <link rel="stylesheet" type="text/css" href="assets/ducksboard-gridster.js-2603104/dist/jquery.gridster.css" />
                        <link rel="stylesheet" type="text/css" href="assets/ducksboard-gridster.js-2603104/dist/demo.css" />
                             
                        <!--section class="Form2Document"-->
                                                       
                        <div class="span8 " style="border: 1px solid #E9E9E9;">
                            <div id="main-column" style="height: 500px;">
                                <h1 id="form-name" style="border-bottom-width: 1px; border-style: none none solid; border-bottom-color: rgb(239, 239, 239); margin: 8px 15px;">New Form</h1>
                                <form id="dynamicform" action="" runat="server" method="get" enctype="multipart/form-data" class="ui-sortable"> <!--Form-->
                                    <!--Makes Web Service available for the Client Script -->                                  
                                    <asp:ScriptManager ID="ScriptManager1" runat="server">  
                                        <Services>  
                                            <asp:ServiceReference Path="ws\EP-WebService.asmx" />  
                                        </Services>  
                                    </asp:ScriptManager> 
                                                        
                                                   
                                    <!-- START GRIDSTER COTAINER -->     
                                    <div style="position: relative;float:left;  height:842px; width:644px; overflow:hidden; background: #fff;
                                        box-sizing:border-box;
                                        -moz-box-sizing:border-box;
                                        -webkit-box-sizing:border-box;
                                        border: 1px solid #000;">
                                        <div style="float:none; position:absolute; width:100%; height:100%;">
                                            <svg width="100%" height="100%" xmlns="http://www.w3.org/2000/svg">
                                                <defs>
                                                    <pattern id="smallGrid" width="8" height="8" patternUnits="userSpaceOnUse">
                                                        <path d="M 8 0 L 0 0 0 8" fill="none" stroke="gray" stroke-width="0.5"/>
                                                    </pattern>
                                                <pattern id="grid" width="80" height="80" patternUnits="userSpaceOnUse">
                                                    <rect width="80" height="80" fill="url(#smallGrid)"/>
                                                    <path d="M 80 0 L 0 0 0 80" fill="none" stroke="gray" stroke-width="1"/>
                                                </pattern>
                                                </defs>
                                                <rect width="100%" height="100%" fill="url(#grid)" />
                                            </svg>

                                        </div>
                                        <div class="gridster">
                                        <ul style="position: relative; height: 760px;">
                                     
                                                <li data-col="1" data-row="1" data-sizex="5" data-sizey="10" class="gs-w unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="6" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px; position: absolute;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="11" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="16" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="21" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="26" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="31" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="36" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="41" data-row="1" data-sizex="5" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="1" data-row="11" data-sizex="45" data-sizey="5" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="1" data-row="16" data-sizex="20" data-sizey="5" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="21" data-row="16" data-sizex="25" data-sizey="5" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="1" data-row="21" data-sizex="15" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="16" data-row="21" data-sizex="15" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="31" data-row="21" data-sizex="15" data-sizey="10" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="1" data-row="31" data-sizex="45" data-sizey="1" class="gs-w unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="1" data-row="32" data-sizex="15" data-sizey="9" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="16" data-row="32" data-sizex="5" data-sizey="9" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px; position: absolute;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="21" data-row="32" data-sizex="10" data-sizey="9" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="31" data-row="32" data-sizex="2" data-sizey="2" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="33" data-row="32" data-sizex="2" data-sizey="2" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="35" data-row="32" data-sizex="2" data-sizey="2" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="37" data-row="32" data-sizex="2" data-sizey="2" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="39" data-row="32" data-sizex="2" data-sizey="2" class="gs-w  player-revert unselected" style="display: list-item; position: absolute; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="41" data-row="32" data-sizex="5" data-sizey="5" class="gs-w  unselected" style="display: list-item; position: absolute;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="31" data-row="34" data-sizex="1" data-sizey="1" class="gs-w  unselected" style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li><li data-col="32" data-row="34" data-sizex="1" data-sizey="1" class="gs-w " style="display: list-item; min-width: 16px; min-height: 16px;"><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li></ul></ul>
                                        </div>

                                    </div>
                                    <!-- END OF GRIDSTER CONTAINER -->
                                
                                </form>
                            </div>
                        </div>
                        <!--/section-->
                        <script type="text/javascript" src="assets/js/jquery-1.7.2.min.js"></script>
                        <script type="text/javascript" src="assets/ducksboard-gridster.js-2603104/src/jquery.coords.js" charster="utf-8"></script>
                        <script type="text/javascript" src="assets/ducksboard-gridster.js-2603104/src/jquery.gridster.js" charster="utf-8"></script>
  
                        <script type="text/javascript">
                            var gridster;

                            $(function () {
                                gridster = $(".gridster > ul").gridster({
                                    widget_margins: [0, 0],
                                    widget_base_dimensions: [8, 8],
                                    min_cols: 8,
                                    max_cols: 80,

                                    resize: { enabled: true }
                                }).data('gridster');
                            });


                            $('#add_widget').on('click', function () {
                                gridster.add_widget('<li></li>', 5, 5)
                            });

        

                            //$('#saveform').on('click', function () {
                            //    gridster.add_widget('<li><span class="gs-resize-handle gs-resize-handle-x"></span><span class="gs-resize-handle gs-resize-handle-y"></span><span class="gs-resize-handle gs-resize-handle-both"></span></li>', 5, 5)
                            //});

                                 </script> 
                        <script type="text/javascript">
                            var gridwidgets = new Array();

                            function widget(_widgetid, _datacol, _datarow, _datasizex, _datasizey, _class) {
                                this._WidgetID = _widgetid;
                                this._DataCol = _datacol;
                                this._DataRow = _datarow;
                                this._DataSizeX = _datasizex;
                                this._DataSizeY = _datasizey;
                                this._Class = _class;
                            }

                            function saveGrid() {
                                SaveWidgetsFromGrid();
                               // CMS.ws.EP_WebService.SaveGrid(gridwidgets, SucceededCallback, OnError);
                                CMS.ws.EP_WebService.SaveGrid("Oracle", "", "Sample Grid", gridwidgets, SucceededCallback, OnError);
                                
                            }

                            function SucceededCallback(result, eventArgs) {
                                //var RsltElem = document.getElementById("divStatusMessage");
                                //RsltElem.innerHTML = result;
                                alert("Works!");
                            }

                            function OnError(error) {
                                alert("Service Error: " + error.get_message());
                            }

                            function SaveWidgetsFromGrid() {
                                //gridwidgets = new Array();

                                var lis = document.getElementsByClassName('gs-w');

                                for (var i = 0, im = lis.length; im > i; i++)
                                    gridwidgets[gridwidgets.length++] = new widget(i, lis[i].getAttribute('data-col'),
                                        lis[i].getAttribute('data-row'),
                                        lis[i].getAttribute('data-sizex'),
                                        lis[i].getAttribute('data-sizey'),
                                        lis[i].getAttribute('class'));
            
                            }

                            $('#saveform').live('click', function () {
                                jQuery('li.gs-w').removeClass('selected');
                                jQuery('li.gs-w').addClass('unselected');
                                saveGrid();     
                            });

                          </script> 


                        </div>
                    </div>
                </div>
                <!-- END PAGE CONTENT-->
            </div>
            <!-- END PAGE CONTAINER-->
        </div>
        <!-- END PAGE -->
    </div>
    <!-- END CONTAINER -->
    <theme:footer ID="footer" runat="server" />
    <!-- BEGIN JAVASCRIPTS -->
    <!-- Load javascripts at bottom, this will reduce page load time -->
    <!--script src="assets/js/jquery-1.8.3.min.js"></!--script-->
    <script src="assets/breakpoints/breakpoints.js"></script>
    <script src="assets/bootstrap/js/bootstrap.min.js"></script>
    <script src="assets/bootstrap-wizard/jquery.bootstrap.wizard.min.js"></script>
    <script src="assets/js/jquery.blockui.js"></script>
    <script src="assets/js/jquery.cookie.js"></script>
    <script src="assets/js/jquery-ui-1.8.23.custom.min.js"></script>
    <!-- ie8 fixes -->
    <!--[if lt IE 9]>
   <script src="assets/js/excanvas.js"></script>
   <script src="assets/js/respond.js"></script>
   <![endif]-->
    <script type="text/javascript" src="assets/chosen-bootstrap/chosen/chosen.jquery.min.js"></script>
    <script type="text/javascript" src="assets/uniform/jquery.uniform.min.js"></script>
    <script type="text/javascript" src="assets/bootstrap-wysihtml5/wysihtml5-0.3.0.js"></script>
    <script type="text/javascript" src="assets/bootstrap-wysihtml5/bootstrap-wysihtml5.js"></script>
    <script type="text/javascript" src="assets/bootstrap-toggle-buttons/static/js/jquery.toggle.buttons.js"></script>
    <script type="text/javascript" src="assets/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="assets/bootstrap-daterangepicker/date.js"></script>
    <script type="text/javascript" src="assets/bootstrap-daterangepicker/daterangepicker.js"></script>
    <script type="text/javascript" src="assets/bootstrap-colorpicker/js/bootstrap-colorpicker.js"></script>
    <script type="text/javascript" src="assets/bootstrap-timepicker/js/bootstrap-timepicker.js"></script>
    <script src="assets/js/app.js"></script>
  
    <script>
        jQuery(document).ready(function () {
            // initiate layout and plugins
            App.init();

        });

        jQuery(document).ready(function () {
            $("#flow-options-style").change(function () {

                var a = $("#flow-options-style option:selected").attr("id");

                $("link#csstheme").attr("href", "assets/flow-css/" + a + ".css")
            });


        });
    </script>
    <script type="text/javascript" src="assets/js/jquery.metadata.js"></script>
    <script type="text/javascript" src="assets/js/jquery.validate.js"></script>
    <script type="text/javascript" src="assets/js/jquery.tipsy.js"></script>
    <script type="text/javascript" src="assets/js/jquery.json-2.3.min.js"></script>
    <script type="text/javascript" src="assets/js/jQuery.fileinput.js"></script>
     <script src="assets/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script type="text/javascript" src="assets/js/flow/main.min.js"></script>
    <script type="text/javascript" src="assets/js/flow/flow.min.js"></script>
   
    <script type="text/javascript">

        $(document).ready(function() 
        {
            jQuery('li.gs-w').live('click', function () {
            jQuery('li.gs-w').removeClass('selected');
            jQuery('li.gs-w').addClass('unselected');
            jQuery(this).removeClass('unselected');
            jQuery(this).addClass('selected');
        });
        });

    </script>
    <!-- END JAVASCRIPTS -->
</body>
<!-- END BODY -->
</html>
