$baseUrl = "http://localhost:5205"

Write-Host "================ 1. PROBANDO /api/salud ================"
$salud = Invoke-RestMethod -Uri "$baseUrl/api/salud" -Method Get
Write-Host "Salud:" ($salud | ConvertTo-Json -Compress)

Write-Host "`n================ 2. PROBANDO /api/categorias ================"
$cats = Invoke-RestMethod -Uri "$baseUrl/api/categorias" -Method Get
Write-Host "Categorias existentes:" $cats.Count

$cat1 = Invoke-RestMethod -Uri "$baseUrl/api/categorias/1" -Method Get
Write-Host "GET /api/categorias/1:" $cat1.nombre

$randCatSlug = "rotulos-neon-" + (Get-Random -Maximum 10000)
$nuevaCatJson = '{"nombre":"Rotulos Neon","slug":"' + $randCatSlug + '","descripcion":"Rotulos luminosos neon led","orden":5}'
$catCreada = Invoke-RestMethod -Uri "$baseUrl/api/categorias" -Method Post -Body $nuevaCatJson -ContentType "application/json"
Write-Host "POST /api/categorias - Creada ID:" $catCreada.id $catCreada.nombre

$catPutJson = '{"nombre":"Rotulos Neon LED","slug":"' + $randCatSlug + '-edit","descripcion":"Actualizado","orden":5,"activo":true}'
$catEditada = Invoke-RestMethod -Uri "$baseUrl/api/categorias/$($catCreada.id)" -Method Put -Body $catPutJson -ContentType "application/json"
Write-Host "PUT /api/categorias/$($catCreada.id):" $catEditada.nombre

Write-Host "`n================ 3. PROBANDO /api/auth/login y /api/auth/yo ================"
$loginJson = '{"correo":"cliente1@correo.com","password":"Viza12345"}'
$loginResp = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginJson -ContentType "application/json"
Write-Host "Login exitoso para:" $loginResp.cliente.nombre
$token = $loginResp.token
Write-Host "Token JWT recibido: $($token.Substring(0, 25))..."

$headers = @{ Authorization = "Bearer $token" }
$yo = Invoke-RestMethod -Uri "$baseUrl/api/auth/yo" -Method Get -Headers $headers
Write-Host "GET /api/auth/yo autenticado:" $yo.nombre "(" $yo.email ")"

Write-Host "`n================ 4. PROBANDO /api/clientes ================"
$clientes = Invoke-RestMethod -Uri "$baseUrl/api/clientes" -Method Get
Write-Host "Total clientes:" $clientes.Count

$randomEmail = "cliente" + (Get-Random -Maximum 100000) + "@correo.com"
$nuevoClienteJson = '{"nombre":"Juan Perez Ingenieria","telefono":"76543210","email":"' + $randomEmail + '","password":"Password123"}'
$clienteCreado = Invoke-RestMethod -Uri "$baseUrl/api/clientes" -Method Post -Body $nuevoClienteJson -ContentType "application/json"
Write-Host "POST /api/clientes - Creado ID:" $clienteCreado.id $clienteCreado.nombre "TieneCuenta:" $clienteCreado.tieneCuenta

Write-Host "`n================ 5. PROBANDO /api/trabajos ================"
$trabajos = Invoke-RestMethod -Uri "$baseUrl/api/trabajos" -Method Get
Write-Host "Total trabajos:" $trabajos.Count

$trabajo1 = Invoke-RestMethod -Uri "$baseUrl/api/trabajos/1" -Method Get
Write-Host "GET /api/trabajos/1:" $trabajo1.titulo "(Categoria: $($trabajo1.categoriaNombre))"

$nuevoTrabajoJson = '{"categoriaId":1,"titulo":"Placa Inox Personalizada","descripcion":"Placa grabada para empresa","ciudad":"La Paz","fechaTrabajo":"2026-10-20","destacado":true,"publicado":true,"orden":2,"imagenPortadaUrl":"/uploads/galeria/placa-inox.jpg"}'
$trabajoCreado = Invoke-RestMethod -Uri "$baseUrl/api/trabajos" -Method Post -Body $nuevoTrabajoJson -ContentType "application/json"
Write-Host "POST /api/trabajos - Creado ID:" $trabajoCreado.id $trabajoCreado.titulo

$trabajoPutJson = '{"categoriaId":1,"titulo":"Placa Inox Personalizada Modificada","descripcion":"Actualizada","ciudad":"La Paz","fechaTrabajo":"2026-10-21","destacado":true,"publicado":true,"orden":3}'
$trabajoModificado = Invoke-RestMethod -Uri "$baseUrl/api/trabajos/$($trabajoCreado.id)" -Method Put -Body $trabajoPutJson -ContentType "application/json"
Write-Host "PUT /api/trabajos/$($trabajoCreado.id):" $trabajoModificado.titulo

Write-Host "`n================ 6. PROBANDO /api/pedidos ================"
$pedidos = Invoke-RestMethod -Uri "$baseUrl/api/pedidos" -Method Get
Write-Host "Total pedidos:" $pedidos.Count

$pedido1 = Invoke-RestMethod -Uri "$baseUrl/api/pedidos/1" -Method Get
Write-Host "GET /api/pedidos/1:" $pedido1.descripcion "Estado:" $pedido1.estado "Propuestas:" $pedido1.propuestas.Count "Historial:" $pedido1.historial.Count

$nuevoPedidoJson = '{"clienteId":' + $clienteCreado.id + ',"categoriaId":1,"descripcion":"Fabricacion de letrero corporativo en acero inoxidable","fechaEntregaEstimada":"2026-11-15"}'
$pedidoCreado = Invoke-RestMethod -Uri "$baseUrl/api/pedidos" -Method Post -Body $nuevoPedidoJson -ContentType "application/json"
Write-Host "POST /api/pedidos - Creado ID:" $pedidoCreado.id "Estado:" $pedidoCreado.estado

Write-Host "`n================ TODOS LOS ENDPOINTS COMPLETADOS CON EXITO ================"
