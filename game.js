const canvas=document.getElementById('screen');
const ctx=canvas.getContext('2d');
const menu=document.getElementById('menu');
const dialogue=document.getElementById('dialogue');
const speaker=document.getElementById('speaker');
const text=document.getElementById('text');
const start=document.getElementById('start');
const cont=document.getElementById('continue');

const state={x:320,y:330,petalX:450,petalY:180,started:false,metPetal:false,looked:false,steps:0};
let lines=[];let lineIndex=0;

function say(who,msg){lines=[{who,msg}];lineIndex=0;speaker.textContent=who;text.textContent=msg;dialogue.hidden=false;}
function draw(){
  ctx.fillStyle='#111';ctx.fillRect(0,0,640,480);
  // original underground room
  ctx.fillStyle='#202020';ctx.fillRect(40,40,560,390);
  ctx.strokeStyle='#555';ctx.lineWidth=4;ctx.strokeRect(40,40,560,390);
  ctx.fillStyle='#171717';ctx.fillRect(80,80,480,300);
  // cracks
  ctx.strokeStyle='#393939';ctx.beginPath();ctx.moveTo(130,90);ctx.lineTo(180,160);ctx.lineTo(150,230);ctx.moveTo(500,100);ctx.lineTo(450,150);ctx.lineTo(480,250);ctx.stroke();
  // petal
  ctx.fillStyle='#f0c84b';ctx.beginPath();ctx.arc(state.petalX,state.petalY,18,0,Math.PI*2);ctx.fill();
  for(let i=0;i<6;i++){let a=i*Math.PI/3;ctx.fillStyle='#d97735';ctx.beginPath();ctx.ellipse(state.petalX+Math.cos(a)*20,state.petalY+Math.sin(a)*20,9,15,a,0,Math.PI*2);ctx.fill();}
  // player
  ctx.fillStyle='#74a9ff';ctx.fillRect(state.x-10,state.y-10,20,20);
  ctx.fillStyle='#fff';ctx.font='14px monospace';ctx.fillText('WASD / ARROWS',48,460);
}
function move(){if(dialogue.hidden&&state.started){const keys=held;let dx=(keys.ArrowRight||keys.d?1:0)-(keys.ArrowLeft||keys.a?1:0);let dy=(keys.ArrowDown||keys.s?1:0)-(keys.ArrowUp||keys.w?1:0);state.x=Math.max(65,Math.min(575,state.x+dx*3));state.y=Math.max(65,Math.min(370,state.y+dy*3));state.steps++;if(Math.hypot(state.x-state.petalX,state.y-state.petalY)<45&&!state.metPetal){state.metPetal=true;say('THE LAST PETAL','You finally made it here.\nI remember every step you took to find me.');} }draw();requestAnimationFrame(move)}
const held={};addEventListener('keydown',e=>{held[e.key]=true;if(e.key==='Enter'&&!dialogue.hidden)advance()});addEventListener('keyup',e=>held[e.key]=false);
function advance(){dialogue.hidden=true;if(state.metPetal&&state.steps>0&&!state.looked){state.looked=true;say('THE LAST PETAL','Do not trust the room behind you.\nIt changes when you stop looking at it.');}}
cont.onclick=advance;
start.onclick=()=>{menu.hidden=true;state.started=true;say('THE LAST PETAL','Hello.\nI am the last petal left in this place.\nMove closer.');draw();move();};
draw();
