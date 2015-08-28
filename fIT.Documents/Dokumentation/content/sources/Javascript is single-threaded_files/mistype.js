$(document).ready(function(){if(document.domain.substr(-3)=='.ru'){var strings={toolarge:"Слишком большой участок текста",toosmall:"Слишком маленький участок текста",mistype:"Опечатка",askMessage:"Отослать извещение об опечатке в <i>&quot;%%&quot;</i> автору этого материала?<br>Пожалуйста, не присылайте опечатки в комментариях.",askTitle:"Отослать?",sentMessage:"Извещение отправлено. Спасибо!",sentTitle:"Извещение отправлено"}}else{strings={toolarge:"Too much text, please select more precisely",toosmall:"Selected text is too small",mistype:"Typo",askMessage:"Notify the author about a mistype in: <i>&quot;%%&quot;</i> ?<br>Please, don't report mistypes in comments.",askTitle:"Send out?",sentMessage:"The notification is sent. Thanks!",sentTitle:"Notification sent."}}
var selected,selectedNode;var isCtrl=false;$(document).keyup(function(e){if(e.which==17){isCtrl=false;}}).keydown(function(e){if(e.which==17){isCtrl=true;if(document.selection){var range=document.selection.createRange()
if(range){selected=range.text;selectedNode=range.parentElement()}}else if(window.getSelection){selected=window.getSelection()
selectedNode=selected&&selected.anchorNode}else if(document.getSelection){selected=document.getSelection()
selectedNode=selected&&selected.anchorNode}
arguments.callee.selected=(selected+'').replace(/\\/g,"\\\\").replace(/\r?\n/g,"\\n").replace(/"/g,'\\"');arguments.callee.selectedNode=selectedNode}
if(e.which==13&&isCtrl==true){e.cancelBubble=true;e.returnValue=false;if(e.stopPropagation){e.stopPropagation();e.preventDefault();}
selected=arguments.callee.selected;if(selected.length==0){return;}
if(selected.length>200){jAlert(strings.toolarge,strings.mistype);return;}
if(selected.length<5){jAlert(strings.toosmall,strings.mistype);return;}
var nid=window.nid||0
selectedNode=arguments.callee.selectedNode
while(selectedNode){var found=selectedNode.id&&(selectedNode.className+'').match(/node-nid-(\d+)/)
if(found){nid=found[1]
break}
selectedNode=selectedNode.parentNode}
var ask=strings.askMessage.replace('%%',selected)
jConfirm(ask,strings.askTitle,function(yes){if(!yes)return
Drupal.service('mistype.save',{mistype:selected,url:window.location.href+'',nid:nid},function(status,data){})
jAlert(strings.sentMessage,strings.sentTitle);})}});});