Drupal.dhtmlMenu={};Drupal.behaviors.dhtmlMenu=function(){if(Drupal.dhtmlMenu.init){return;}
else{Drupal.dhtmlMenu.init=true;}
var effects=Drupal.settings.dhtmlMenu;$('.collapsed').removeClass('expanded');if(!effects.siblings){var cookie=Drupal.dhtmlMenu.cookieGet();for(var i in cookie){var li=$('#dhtml_menu-'+ cookie[i]).parents('li:first');if($(li).hasClass('collapsed')){Drupal.dhtmlMenu.toggleMenu(li);}}}
$('ul.menu li.dhtml-menu:not(.leaf,.no-dhtml)').each(function(){var li=this;if(effects.clone){var ul=$(li).find('ul:first');if(ul.length){$(li).find('a:first').clone().prependTo(ul).wrap('<li class="leaf fake-leaf"></li>');}}
if(effects.doubleclick){$(li).find('a:first').dblclick(function(e){window.location=this.href;});}
$(li).find('a:first').click(function(e){Drupal.dhtmlMenu.toggleMenu($(li));return false;});});}
Drupal.dhtmlMenu.toggleMenu=function(li){var effects=Drupal.settings.dhtmlMenu;if($(li).hasClass('expanded')){if(effects.slide){$(li).find('ul:first').animate({height:'hide',opacity:'hide'},'1000');}
else $(li).find('ul:first').css('display','none');if(effects.children){if(effects.slide){$(li).find('li.expanded').find('ul:first').animate({height:'hide',opacity:'hide'},'1000');}
else $(li).find('li.expanded').find('ul:first').css('display','none');$(li).find('li.expanded').removeClass('expanded').addClass('collapsed')}
$(li).removeClass('expanded').addClass('collapsed');}
else{if(effects.slide){$(li).find('ul:first').animate({height:'show',opacity:'show'},'1000');}
else $(li).find('ul:first').css('display','block');$(li).removeClass('collapsed').addClass('expanded');if(effects.siblings){var id=$(li).find('a:first').attr('id');$(li).find('li').addClass('own-children-temp');if(effects.relativity){var siblings=$(li).parent().find('li.expanded').not('.own-children-temp').not(':has(#'+ id+')');}
else{var siblings=$('ul.menu li.expanded').not('.own-children-temp').not(':has(#'+ id+')');}
if(!effects.children){$('li.collapsed li.expanded').addClass('sibling-children-temp');$(siblings).find('li.expanded').addClass('sibling-children-temp');siblings=$(siblings).not('.sibling-children-temp');}
$('.own-children-temp, .sibling-children-temp').removeClass('own-children-temp').removeClass('sibling-children-temp');if(effects.slide){$(siblings).find('ul:first').animate({height:'hide',opacity:'hide'},'1000');}
else $(siblings).find('ul:first').css('display','none');$(siblings).removeClass('expanded').addClass('collapsed');}}
Drupal.dhtmlMenu.cookieSet();}
Drupal.dhtmlMenu.cookieGet=function(){var c=/dhtml_menu=(.*?)(;|$)/.exec(document.cookie);if(c){return c[1];}
else return'';}
Drupal.dhtmlMenu.cookieSet=function(){var expanded=new Array();$('li.expanded').each(function(){expanded.push($(this).find('a:first').attr('id').substr(5));});document.cookie='dhtml_menu='+ expanded.join(',')+';path=/';}