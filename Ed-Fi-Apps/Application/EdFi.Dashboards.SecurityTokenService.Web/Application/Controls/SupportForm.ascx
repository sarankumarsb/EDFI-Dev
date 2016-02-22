<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SupportForm.ascx.cs" Inherits="EdFi.Dashboards.SecurityTokenService.Web.Controls.SupportForm" %>
<%@ Import Namespace="EdFi.Dashboards.Resources.Navigation" %>

<div id="dialog-form" title="Submit Feedback" style="display: none; height: 331px; min-height: 24px; width: 540px;">
		<p class="validateTips">Please provide the information requested below to submit your feedback.</p>
		<div class="feedback-form">
				<p>
					<label for="submitFeedbackName">Name:</label>
					<input type="text" name="name" id="submitFeedbackName" <%= DisableFeedbackName %> class="text ui-widget-content ui-corner-all" value="<%= UserName %>" />
				</p>
				<p>
					<label for="submitFeedbackEmail">Email:</label>
					<input type="text" name="email" id="submitFeedbackEmail" <%= DisableFeedbackEmail %> class="text ui-widget-content ui-corner-all" value="<%= Email %>" />
				</p>
				<p>
					<label for="submitFeedbackPhone">Phone:</label>
					<input type="text" name="phone" id="submitFeedbackPhone" class="text ui-widget-content ui-corner-all" />
				</p>
				<p>
					<label for="submitFeedbackSubject">Subject:</label>
					<input type="text" name="subject" id="submitFeedbackSubject"  class="text ui-widget-content ui-corner-all"/>
				</p>
				<p>
					<label for="submitFeedbackIssue">Issue:</label>
                    
                        <select name="issue" id="submitFeedbackIssue" class="ui-widget-content ui-corner-all">
                            <option>-Select option-</option>
                            <option>Bug/System Issue</option>
                            <option>Comment</option>
                            <option>Data Problem</option>
                            <option>Feature Request</option>
                            <option>Question</option>
                            <option>Privacy Issue</option>
                            <option>Unable to Login</option>
                        </select>
                        <span id="validationSpanForIssue" style="color:Red;">*</span>
                    
				</p>
				<br/>
				<p><label for="submitFeedbackFeedback">Detailed Description</label></p>
				<p>
					<textarea rows="13" cols="60" name="feedback" id="submitFeedbackFeedback" class="text ui-widget-content ui-corner-all" style="width:100%; height:auto;"></textarea>
				</p>
			</div>
            <div id="feedbackWait" style="display:none;">
                <img src="<%= EdFiWebFormsDashboards.Site.Common.ThemeImage("loadingSmall.gif").Resolve() %>" alt="Submitting..." /> Submitting request...
            </div>
	</div>

     <script type="text/javascript">
         var feedbackSubmitProcessing;
         $(document).ready(function () {
             var $dialog = $('#dialog-form').dialog({ autoOpen: false,
                 height: 520,
                 width: 581,
                 title: 'Submit Request',
                 modal: true,
                 buttons: {
                     'Submit Request': function () {
                         if (feedbackSubmitProcessing)
                             return;

                         //Validate the form data...
                         if (!validateSubmit()) {
                             feedbackSubmitProcessing = false;
                             return;
                         }

                         $('#feedbackWait').show();
                         var data = 'name=' + $('#submitFeedbackName').val() + '&email=' + $('#submitFeedbackEmail').val() + '&phoneNumber=' + $('#submitFeedbackPhone').val() + '&subject=' + $('#submitFeedbackSubject').val() + '&issue=' + $('#submitFeedbackIssue').val() + '&feedback=' + $('#submitFeedbackFeedback').val();

                         $.ajax({ type: 'POST',
                             url: '<%= EdFiWebFormsDashboards.Site.Common.Feedback().Resolve() %>?lea=<%= Server.HtmlEncode(Request.Params["lea"])  %>',
                             data: data,
                             success: function () {
                                 alert('Thank you for your feedback.');
                                 $dialog.dialog('close');
                                 feedbackSubmitProcessing = false;
                                 $('#feedbackWait').hide();
                                 $('#submitFeedbackFeedback').attr('value', '');
                                 $('#submitFeedbackSubject').attr('value', '');
                             },
                             error: function () {
                                 alert('There was an error processing your feedback.');
                                 feedbackSubmitProcessing = false;
                                 $('#feedbackWait').hide();
                             },
                             dataType: 'text'
                         });
                     },
                     'Cancel Request': function () { $dialog.dialog('close'); }
                 }
             });

             $('#submitFeedbackFeedback').focus(function () {
                 feedbackDeactivateDefaultText();
             });

             $('#submitFeedbackFeedback').blur(function () {
                 feedbackActivateDefaultText();
             });

             if ($('#submitFeedbackFeedback').val() == '')
                 feedbackActivateDefaultText();

             $('<%= SupportLinkControlId %>').click(function () {
                 feedbackActivateDefaultText();
                 $dialog.dialog('open');
                 // prevent the default action, e.g., following a link
                 return false;
             });
             
             <% if (!String.IsNullOrWhiteSpace(SupportLinkControlId2))
                { %>

             $('<%= SupportLinkControlId2 %>').click(function () {
                 feedbackActivateDefaultText();
                 $dialog.dialog('open');
                 // prevent the default action, e.g., following a link
                 return false;
             });
             <% } %>

             //For the select for Issue.
             $('#submitFeedbackIssue').change(function () {
                 var selectForIssue = $('#submitFeedbackIssue');
                 var validationSpanForIssue = $('#validationSpanForIssue');

                 if (selectForIssue.val() == "-Select option-")
                     validationSpanForIssue.show();
                 else
                     validationSpanForIssue.hide();
             });
         });

         var feedbackDefaultText = "Please provide more detail and be as specific as possible.";
         var feedbackPageTitle = document.title;
         function feedbackActivateDefaultText() {
             if ($('#submitFeedbackFeedback').val() == '') {
                 $('#submitFeedbackFeedback').val(feedbackDefaultText);
                 $('#submitFeedbackFeedback').addClass("defaultTextActive");
             }
             if ($('#submitFeedbackSubject').val() == '') {
                 $('#submitFeedbackSubject').val(feedbackPageTitle);
             }
         }

         function feedbackDeactivateDefaultText() {
             if ($('#submitFeedbackFeedback').val() == feedbackDefaultText) {
                 $('#submitFeedbackFeedback').val('');
                 $('#submitFeedbackFeedback').removeClass("defaultTextActive");
             }
         }

         function validateSubmit() {
             //For the select that sets the Issue Type.
             var selectValue = $('#submitFeedbackIssue').val();
             if (selectValue == "-Select option-") {
                 $('#validationSpanForIssue').show();
                 alert("Please select a valid \"Issue\".");
                 return false;
             }

             //For the textarea
             //var detailedDescription = $('#submitFeedbackFeedback').val();

             //Other validations...

             return true;
         }
    </script>
