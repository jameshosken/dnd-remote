
  console.log('Starting Server');

// const localtunnel = require('localtunnel'); // expose port to the world
const ngrok = require('ngrok');

  var app = require('express')();
var http = require('http').createServer(app);
var io = require('socket.io')(http);


console.log('Requirements loaded');


let ngrok_url = '';

app.get('/', function(req, res){
  res.sendFile(__dirname + '/index.html');
});

io.on('connection', function(socket){

  console.log('a user connected');

  socket.on('request-ngrok-url', function(data){
    
    console.log('URL requested, sending: ' + ngrok_url);
    socket.emit('ngrok-url', {url: ngrok_url});
  });

  socket.on('dm-update-map', function(data){
    io.emit('update-map', data);
    console.log(data);
  });

  socket.on('dm-update-mesh', function(data){
    io.emit('update-mesh', data);
    console.log(data);
  });

  socket.on('dm-update-mesh-all', function(data){
    io.emit('update-mesh-all', data);
    console.log(data);
  });
  socket.on('request-mesh-data', function(){
    io.emit('request-mesh-data');
    console.log("Mesh Data Requested");
  });

  socket.on('disconnect', function(){
    console.log('user disconnected');
  });
});

//Check if port busy
http.once('error', function(err) {
  if (err.code === 'EADDRINUSE') {
    console.log('port in use');
    http.close();
    setTimeout(() => {
      
      console.log("bye felicia")
    }, 1000);
  }
});

(async function() {
  ngrok_url = await ngrok.connect(9090);
  console.log('ngrok url: ' + ngrok_url);

})();

// (async () => {
//   const tunnel = await localtunnel({ port: 3000 });

//   console.log("Tunnel: " + tunnel.url);

//   tunnel.on('close', () => {
//     // tunnels are closed
//     console.log('Tunnel closed, goodbye!')
//   });
// })();

http.listen(9090, function(){
  console.log('listening on *:9090');
});

