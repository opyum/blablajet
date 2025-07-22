const express = require('express');
const cors = require('cors');

const app = express();
const PORT = 5000;

// Middleware
app.use(cors());
app.use(express.json());

// Mock data
const flights = [
  {
    id: '1',
    flightNumber: 'EL001',
    departureAirport: { code: 'CDG', name: 'Charles de Gaulle', city: 'Paris' },
    arrivalAirport: { code: 'NCE', name: "Côte d'Azur", city: 'Nice' },
    departureTime: '2024-03-15T14:30:00Z',
    arrivalTime: '2024-03-15T16:00:00Z',
    currentPrice: 1200,
    availableSeats: 6,
    aircraft: { model: 'Citation CJ3+', type: 'Light Jet' },
    company: { name: 'AirLuxe', logoUrl: '/api/placeholder/40/40' }
  },
  {
    id: '2',
    flightNumber: 'EL002',
    departureAirport: { code: 'LHR', name: 'Heathrow', city: 'Londres' },
    arrivalAirport: { code: 'GVA', name: 'Geneva', city: 'Genève' },
    departureTime: '2024-03-16T10:15:00Z',
    arrivalTime: '2024-03-16T13:45:00Z',
    currentPrice: 2100,
    availableSeats: 8,
    aircraft: { model: 'Falcon 2000', type: 'Mid-Size Jet' },
    company: { name: 'SwissJet', logoUrl: '/api/placeholder/40/40' }
  }
];

// Routes
app.get('/', (req, res) => {
  res.json({ message: 'Empty Legs Mock API is running!' });
});

app.get('/health', (req, res) => {
  res.json({ status: 'Healthy', timestamp: new Date().toISOString() });
});

app.get('/api/flights', (req, res) => {
  res.json(flights);
});

app.get('/api/flights/:id', (req, res) => {
  const flight = flights.find(f => f.id === req.params.id);
  if (flight) {
    res.json(flight);
  } else {
    res.status(404).json({ error: 'Flight not found' });
  }
});

app.post('/api/flights/search', (req, res) => {
  const { departure, arrival, date } = req.body;
  // Simple search - return all flights for demo
  res.json(flights);
});

// Start server
app.listen(PORT, () => {
  console.log(`🚀 Mock API running on http://localhost:${PORT}`);
  console.log(`📊 Health check: http://localhost:${PORT}/health`);
  console.log(`✈️  Flights endpoint: http://localhost:${PORT}/api/flights`);
});