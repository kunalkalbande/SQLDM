

var origWidth = $(window).width();
var origHeight = $(window).height();

var resizeMainDashboardWidgets = function () {
	var totalpad = 0; 
	$('.widgetHolder').children('.z-vlayout-inner').each(function(){
		totalpad = totalpad + parseFloat($(this).css('padding-top')) 
							+ parseFloat($(this).css('padding-bottom'));
	});
	var widgetHolderHeight = $('.widgetHolder').height()*$(window).height()/origHeight;
	var widgetHeight = (widgetHolderHeight - totalpad)/3;	

	$('.widgetHolder').find('.z-grid-body').each(function () {
		var panelHeight = 0;
		$(this).parents('.z-panel-body').siblings().each(function () {
			panelHeight += $(this).height(); 
		}); 
		var panelContainer = $(this).parents('.z-panel-body').parent();
		var panelMargin = parseFloat(panelContainer.css('margin-bottom')) + parseFloat(panelContainer.css('margin-top')); 
		
		var headerHeight = $(this).siblings().height();
		$(this).css('height', (widgetHeight - headerHeight - panelHeight - panelMargin) + 'px');
		$(this).parent().css('height', (widgetHeight - panelHeight - panelMargin) + 'px');
	});
};

var resizeMainDashboardCenter = function () {
	var totalpad = 0; 
	var vlayouts = $('.z-center').children().children().children();
	
	vlayouts.each(function(){
		totalpad = totalpad + parseFloat($(this).css('padding-top')) 
							+ parseFloat($(this).css('padding-bottom'));
	});
	
	var instancesListHolder = $('.instancesLayout');
	var instancesListBody = instancesListHolder.find('.z-listbox-body');
	var instancesListVLayoutHeight = $(vlayouts[0]).height();
	var instancesListHeight = instancesListHolder.height();
	var controlsHeight = instancesListVLayoutHeight - instancesListHeight;
	var instancesListHeaderHeight = instancesListBody.siblings().height();
	
	var newCenterHolderHeight = $('.z-center').height()*$(window).height()/origHeight;
	var newInstancesListHolderHeight = (newCenterHolderHeight - totalpad)/2 - controlsHeight;	
	var newInstancesListBodyHeight = newInstancesListHolderHeight - instancesListHeaderHeight;
	
	instancesListBody.css('height', newInstancesListBodyHeight + 'px');
	instancesListBody.parent().css('height', (newInstancesListHolderHeight) + 'px');
};

$(window).resize(function () {
	resizeMainDashboardWidgets();	
	resizeMainDashboardCenter();
	
	origWidth = $(window).width();
	origHeight = $(window).height();	
});				
			
$(window).load(function () {
	resizeMainDashboardWidgets();	
	resizeMainDashboardCenter();
	
	origWidth = $(window).width();
	origHeight = $(window).height();	
});

function f_Copy_ClipBoard() {

    alert("hello amit jain");
    
    if (window.clipboardData) {
    window.clipboardData.setData("Text");
       }
}

function Hide()
{
	alert("hide");
   //alerts-grid-div.style.display="none";
}
