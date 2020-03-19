
  console.log('Starting Server');
var app = require('express')();
var http = require('http').createServer(app);
var io = require('socket.io')(http);

console.log('Requirements loaded');

console.log(io);


app.get('/', function(req, res){
  res.sendFile(__dirname + '/index.html');
});

io.on('connection', function(socket){

  console.log('a user connected');

  socket.on('test-message', function(data){
    console.log(data);
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

http.listen(3000, function(){
  console.log('listening on *:3000');
  
});