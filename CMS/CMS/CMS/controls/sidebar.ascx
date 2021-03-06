﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sidebar.ascx.cs" Inherits="CMS.controls.sidebar" %>

<!-- BEGIN SIDEBAR -->
		<div class="page-sidebar nav-collapse collapse">
			<!-- BEGIN SIDEBAR MENU -->        	
			<ul>
				<li>
					<!-- BEGIN SIDEBAR TOGGLER BUTTON -->
					<div class="sidebar-toggler hidden-phone"></div>
					<!-- BEGIN SIDEBAR TOGGLER BUTTON -->
				</li>
				<li>
					<!-- BEGIN RESPONSIVE QUICK SEARCH FORM -->
					<form class="sidebar-search">
						<div class="input-box">
							<a href="javascript:;" class="remove"></a>
							<input type="text" placeholder="Search..." />				
							<input type="button" class="submit" value=" " />
						</div>
					</form>
					<!-- END RESPONSIVE QUICK SEARCH FORM -->
				</li>
				<!--<li class="start ">
					<a href="index.aspx">
					<i class="icon-home"></i> 
					<span class="title">Dashboard</span>
					</a>
				</li>-->

                <li class="has-sub">
					<a href="javascript:;">
					<i class="icon-user"></i> 
					<span class="title">Accounts</span>
                    <span class="arrow "></span>
					</a>
                    <ul class="sub">
						<li ><a href="accounts.aspx">All Accounts</a></li>
                        <li ><a href="roles.aspx">Roles & Privileges</a></li>
					</ul>
				</li>
				<li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-bookmark-empty"></i> 
					<span class="title">Applications</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="apps.aspx">All Applications</a></li>

						
					</ul>
				</li>
                  <!--<li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-wrench"></i> 
					<span class="title">Flows</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="flows.aspx">All Flows</a></li>
						<li ><a href="newflow.aspx">New Flow</a></li>
						
					</ul>
				</li>-->
				<li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-table"></i> 
					<span class="title">Forms</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="forms.aspx">All Forms</a></li>
						<li ><a href="newform.aspx">New Form</a></li>
                        <li ><a href="Form2Document.aspx">Form2Document</a></li>
					</ul>
				</li>
                <!--<li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-briefcase"></i> 
					<span class="title">Cases</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="cases.aspx">All Cases</a></li>
						<li ><a href="newcase.aspx">New Case</a></li>
					</ul>
				</li>-->
              
                <li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-bar-chart"></i> 
					<span class="title">Views</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="reports.aspx">All Views</a></li>
						<li ><a href="newreports.aspx">New Views</a></li>
						
					</ul>
				</li>

				<!--<li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-th-list"></i> 
					<span class="title">Pages</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="tasks.aspx">All Tasks</a></li>
                        <li ><a href="newtasks.aspx">New Task</a></li>
						
					</ul>
				</li>-->

                
              <!--   <li class="has-sub ">
					<a href="javascript:;">
					<i class="icon-calendar"></i> 
					<span class="title">Calendars</span>
					<span class="arrow "></span>
					</a>
					<ul class="sub">
						<li ><a href="calendars.aspx">All Calendars</a></li>
						<li ><a href="calendar.aspx">New Calendar</a></li>
						
					</ul>
				</li>-->
				
				
			</ul>
			<!-- END SIDEBAR MENU -->
		</div>
		<!-- END SIDEBAR -->