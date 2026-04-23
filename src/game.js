// Scene setup
const scene = new THREE.Scene();
scene.background = new THREE.Color(0x87ceeb);

const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.shadowMap.enabled = true;
document.body.appendChild(renderer.domElement);

// Lighting
const light = new THREE.DirectionalLight(0xffffff, 1);
light.position.set(50, 50, 50);
light.castShadow = true;
light.shadow.mapSize.width = 2048;
light.shadow.mapSize.height = 2048;
light.shadow.camera.far = 500;
light.shadow.camera.left = -200;
light.shadow.camera.right = 200;
light.shadow.camera.top = 200;
light.shadow.camera.bottom = -200;
scene.add(light);

const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
scene.add(ambientLight);

// Ground
const groundGeometry = new THREE.PlaneGeometry(500, 500);
const groundMaterial = new THREE.MeshLambertMaterial({ color: 0x228b22 });
const ground = new THREE.Mesh(groundGeometry, groundMaterial);
ground.rotation.x = -Math.PI / 2;
ground.receiveShadow = true;
scene.add(ground);

// Player
const playerGeometry = new THREE.CapsuleGeometry(0.5, 2, 4, 8);
const playerMaterial = new THREE.MeshPhongMaterial({ color: 0xff6b6b });
const player = new THREE.Mesh(playerGeometry, playerMaterial);
player.position.set(0, 1, 0);
player.castShadow = true;
player.receiveShadow = true;
scene.add(player);

// Camera offset from player
const cameraOffset = new THREE.Vector3(0, 3, 5);

// Input handling
const keys = {};
window.addEventListener('keydown', (e) => {
    keys[e.key.toLowerCase()] = true;
});
window.addEventListener('keyup', (e) => {
    keys[e.key.toLowerCase()] = false;
});

// Mouse look
let mouseX = 0;
let mouseY = 0;
let targetRotationY = 0;
let targetRotationX = 0;

document.addEventListener('mousemove', (e) => {
    const deltaX = e.movementX || 0;
    const deltaY = e.movementY || 0;
    
    targetRotationY -= deltaX * 0.005;
    targetRotationX -= deltaY * 0.005;
    
    targetRotationX = Math.max(-Math.PI / 2, Math.min(Math.PI / 2, targetRotationX));
});

document.addEventListener('click', () => {
    document.body.requestPointerLock = document.body.requestPointerLock || document.body.mozRequestPointerLock;
    document.body.requestPointerLock();
});

// Player rotation
let playerRotationY = 0;

// Movement
const moveSpeed = 0.3;
const velocity = new THREE.Vector3(0, 0, 0);

function updatePlayer() {
    const forward = new THREE.Vector3(0, 0, -1);
    const right = new THREE.Vector3(1, 0, 0);
    
    forward.applyAxisAngle(new THREE.Vector3(0, 1, 0), playerRotationY);
    right.applyAxisAngle(new THREE.Vector3(0, 1, 0), playerRotationY);
    
    velocity.multiplyScalar(0.8);
    
    if (keys['w']) velocity.add(forward.multiplyScalar(moveSpeed));
    if (keys['s']) velocity.add(forward.multiplyScalar(-moveSpeed));
    if (keys['a']) velocity.add(right.multiplyScalar(-moveSpeed));
    if (keys['d']) velocity.add(right.multiplyScalar(moveSpeed));
    
    player.position.add(velocity);
    
    // Smooth rotation
    playerRotationY += (targetRotationY - playerRotationY) * 0.1;
    player.rotation.y = playerRotationY;
    
    // Update camera position
    const cameraPos = cameraOffset.clone();
    cameraPos.applyAxisAngle(new THREE.Vector3(0, 1, 0), playerRotationY);
    cameraPos.applyAxisAngle(new THREE.Vector3(1, 0, 0), targetRotationX);
    cameraPos.add(player.position);
    
    camera.position.lerp(cameraPos, 0.1);
    camera.lookAt(player.position.clone().add(new THREE.Vector3(0, 1, 0)));
    
    // Update info
    document.getElementById('pos').textContent = 
        `${player.position.x.toFixed(1)}, ${player.position.y.toFixed(1)}, ${player.position.z.toFixed(1)}`;
}

// Handle window resize
window.addEventListener('resize', () => {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
});

// Animation loop
function animate() {
    requestAnimationFrame(animate);
    updatePlayer();
    renderer.render(scene, camera);
}

animate();
