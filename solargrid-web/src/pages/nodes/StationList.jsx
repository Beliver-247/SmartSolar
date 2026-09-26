import React, { useEffect, useState } from 'react';
import { stationsApi } from '../../api/stationsApi';
import { useAuth } from '../../auth/AuthContext';
import Button from '../../components/ui/Button';
import Input from '../../components/ui/Input';
import Modal from '../../components/ui/Modal';
import LoadingSpinner from '../../components/ui/LoadingSpinner';
import ErrorBanner from '../../components/ui/ErrorBanner';
import EmptyState from '../../components/ui/EmptyState';
import toast from 'react-hot-toast';
import { Plus, Edit2, Trash2 } from 'lucide-react';

const StationList = () => {
  const { user } = useAuth();
  const isBackoffice = user?.role === 'Backoffice';
  const canManageSlots = user?.role === 'Backoffice';

  const [stations, setStations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  // Modals
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [isSlotOpen, setIsSlotOpen] = useState(false);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [selectedStation, setSelectedStation] = useState(null);

  // Forms
  const [newStation, setNewStation] = useState({ 
    stationName: '', lat: '', lng: '', capacityKwh: '', batterySlots: '', 
    schedule: [{ start: '', end: '' }] 
  });
  const [slotForm, setSlotForm] = useState({ totalSlots: '' });
  const [editForm, setEditForm] = useState(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchStations();
  }, []);

  const fetchStations = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await stationsApi.getAll();
      setStations(data);
    } catch (err) {
      setError('Failed to load stations.');
    } finally {
      setLoading(false);
    }
  };

  const hasOverlappingSlots = (schedule) => {
    const validSlots = schedule.filter(s => s.start && s.end);
    
    const toMinutes = (timeStr) => {
      const [h, m] = timeStr.split(':').map(Number);
      return h * 60 + m;
    };

    for (let i = 0; i < validSlots.length; i++) {
      const startA = toMinutes(validSlots[i].start);
      const endA = toMinutes(validSlots[i].end);
      
      if (startA >= endA) return "A slot's end time must be after its start time.";
      
      for (let j = i + 1; j < validSlots.length; j++) {
        const startB = toMinutes(validSlots[j].start);
        const endB = toMinutes(validSlots[j].end);
        
        if (Math.max(startA, startB) < Math.min(endA, endB)) {
          return "Schedule contains overlapping time slots.";
        }
      }
    }
    return null;
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    
    const overlapError = hasOverlappingSlots(newStation.schedule);
    if (overlapError) {
      toast.error(overlapError);
      return;
    }

    setSaving(true);
    try {
      const finalSchedule = newStation.schedule
        .filter(s => s.start && s.end)
        .map(s => `${s.start}-${s.end}`);

      const payload = {
        stationName: newStation.stationName,
        gpsLocation: { lat: parseFloat(newStation.lat), lng: parseFloat(newStation.lng) },
        capacityKwh: parseFloat(newStation.capacityKwh),
        batterySlots: parseInt(newStation.batterySlots, 10),
        schedule: finalSchedule
      };
      await stationsApi.create(payload);
      toast.success('Station created successfully');
      setIsCreateOpen(false);
      setNewStation({ stationName: '', lat: '', lng: '', capacityKwh: '', batterySlots: '', schedule: [{ start: '', end: '' }] });
      fetchStations();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to create station');
    } finally {
      setSaving(false);
    }
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    
    const overlapError = hasOverlappingSlots(editForm.schedule);
    if (overlapError) {
      toast.error(overlapError);
      return;
    }

    setSaving(true);
    try {
      const finalSchedule = editForm.schedule
        .filter(s => s.start && s.end)
        .map(s => `${s.start}-${s.end}`);

      const payload = {
        stationName: editForm.stationName,
        gpsLocation: { lat: parseFloat(editForm.lat), lng: parseFloat(editForm.lng) },
        capacityKwh: parseFloat(editForm.capacityKwh),
        batterySlots: parseInt(editForm.batterySlots, 10),
        schedule: finalSchedule
      };
      await stationsApi.update(selectedStation.id, payload);
      toast.success('Station updated successfully');
      setIsEditOpen(false);
      fetchStations();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update station');
    } finally {
      setSaving(false);
    }
  };

  const handleUpdateSlots = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await stationsApi.updateSlots(selectedStation.id, parseInt(slotForm.totalSlots, 10));
      toast.success('Slots updated successfully');
      setIsSlotOpen(false);
      fetchStations();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update slots');
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async (id, name) => {
    if (!window.confirm(`Are you sure you want to deactivate ${name}?`)) return;
    try {
      await stationsApi.deactivate(id);
      toast.success('Station deactivated');
      fetchStations();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to deactivate station. Active reservations may exist.');
    }
  };

  const openSlotModal = (station) => {
    setSelectedStation(station);
    setSlotForm({ totalSlots: station.batterySlots });
    setIsSlotOpen(true);
  };

  const openEditModal = (station) => {
    setSelectedStation(station);
    const parsedSlots = (station.schedule || []).map(s => {
      const [start, end] = s.split('-');
      return { start: start || '', end: end || '' };
    });
    
    setEditForm({
      stationName: station.stationName,
      lat: station.gpsLocation?.lat || '',
      lng: station.gpsLocation?.lng || '',
      capacityKwh: station.capacityKwh,
      batterySlots: station.batterySlots,
      schedule: parsedSlots.length > 0 ? parsedSlots : [{ start: '', end: '' }]
    });
    setIsEditOpen(true);
  };

  const renderScheduleEditor = (formState, setFormState) => (
    <div className="space-y-3 mt-4 border-t pt-4">
      <div className="flex justify-between items-center">
        <label className="text-sm font-medium text-gray-700">Operating Schedule</label>
        <Button 
          type="button" 
          variant="outline" 
          className="text-xs py-1 px-2"
          onClick={() => setFormState({...formState, schedule: [...formState.schedule, { start: '', end: '' }]})}
        >
          <Plus size={14} className="inline mr-1" /> Add Slot
        </Button>
      </div>
      
      {formState.schedule.map((slot, index) => (
        <div key={index} className="flex items-center space-x-2">
          <Input 
            type="time" 
            value={slot.start} 
            onChange={e => {
              const newSched = [...formState.schedule];
              newSched[index].start = e.target.value;
              setFormState({...formState, schedule: newSched});
            }} 
            required
            className="flex-1"
          />
          <span className="text-gray-500">to</span>
          <Input 
            type="time" 
            value={slot.end} 
            onChange={e => {
              const newSched = [...formState.schedule];
              newSched[index].end = e.target.value;
              setFormState({...formState, schedule: newSched});
            }} 
            required
            className="flex-1"
          />
          <button 
            type="button"
            onClick={() => {
              const newSched = formState.schedule.filter((_, i) => i !== index);
              setFormState({...formState, schedule: newSched});
            }}
            className="p-2 text-red-500 hover:bg-red-50 rounded-md transition-colors"
          >
            <Trash2 size={18} />
          </button>
        </div>
      ))}
      {formState.schedule.length === 0 && (
        <p className="text-sm text-gray-500 italic">No schedule slots defined.</p>
      )}
    </div>
  );

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold text-gray-800">Microgrid Nodes (Stations)</h2>
        {isBackoffice && (
          <Button onClick={() => setIsCreateOpen(true)} className="flex items-center space-x-2">
            <Plus size={18} />
            <span>Add Station</span>
          </Button>
        )}
      </div>

      <ErrorBanner message={error} />

      {loading ? (
        <div className="py-12"><LoadingSpinner /></div>
      ) : stations.length === 0 ? (
        <EmptyState title="No Stations Found" description="There are no microgrid nodes available." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {stations.map((station) => (
            <div key={station.id} className="bg-white rounded-lg shadow border border-gray-200 p-6 flex flex-col">
              <div className="flex justify-between items-start mb-4">
                <h3 className="text-lg font-semibold text-gray-900">{station.stationName}</h3>
                {station.isActive ? (
                  <span className="px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">Active</span>
                ) : (
                  <span className="px-2 py-1 text-xs font-semibold rounded-full bg-red-100 text-red-800">Inactive</span>
                )}
              </div>
              
              <div className="space-y-2 flex-1 text-sm text-gray-600">
                <p><span className="font-medium text-gray-900">Capacity:</span> {station.capacityKwh} kWh</p>
                <p><span className="font-medium text-gray-900">Total Battery Slots:</span> {station.batterySlots}</p>
                <p><span className="font-medium text-gray-900">Location:</span> {station.gpsLocation?.lat}, {station.gpsLocation?.lng}</p>
                <p><span className="font-medium text-gray-900">Schedule:</span> {station.schedule?.length ? station.schedule.join(', ') : 'None'}</p>
              </div>

              <div className="mt-6 pt-4 border-t border-gray-100 flex flex-wrap gap-2 justify-end">
                {canManageSlots && (
                  <Button variant="outline" className="text-xs py-1" onClick={() => openSlotModal(station)}>
                    Manage Slots
                  </Button>
                )}
                {isBackoffice && (
                  <Button variant="secondary" className="text-xs py-1" onClick={() => openEditModal(station)}>
                    <Edit2 size={14} className="inline mr-1"/> Edit
                  </Button>
                )}
                {isBackoffice && station.isActive && (
                  <Button variant="danger" className="text-xs py-1" onClick={() => handleDeactivate(station.id, station.stationName)}>
                    Deactivate
                  </Button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Create Station Modal */}
      {isBackoffice && (
        <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Add New Station">
          <form onSubmit={handleCreate} className="space-y-4">
            <Input label="Station Name" value={newStation.stationName} onChange={e => setNewStation({...newStation, stationName: e.target.value})} required />
            <div className="grid grid-cols-2 gap-4">
              <Input label="Latitude" type="number" step="any" value={newStation.lat} onChange={e => setNewStation({...newStation, lat: e.target.value})} required />
              <Input label="Longitude" type="number" step="any" value={newStation.lng} onChange={e => setNewStation({...newStation, lng: e.target.value})} required />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <Input label="Capacity (kWh)" type="number" value={newStation.capacityKwh} onChange={e => setNewStation({...newStation, capacityKwh: e.target.value})} required />
              <Input label="Total Battery Slots" type="number" value={newStation.batterySlots} onChange={e => setNewStation({...newStation, batterySlots: e.target.value})} required />
            </div>
            
            {renderScheduleEditor(newStation, setNewStation)}

            <div className="pt-4 flex justify-end space-x-3 border-t">
              <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={saving}>{saving ? 'Saving...' : 'Create Station'}</Button>
            </div>
          </form>
        </Modal>
      )}

      {/* Edit Station Modal */}
      {isBackoffice && editForm && (
        <Modal isOpen={isEditOpen} onClose={() => setIsEditOpen(false)} title={`Edit Station: ${selectedStation?.stationName}`}>
          <form onSubmit={handleUpdate} className="space-y-4">
            <Input label="Station Name" value={editForm.stationName} onChange={e => setEditForm({...editForm, stationName: e.target.value})} required />
            <div className="grid grid-cols-2 gap-4">
              <Input label="Latitude" type="number" step="any" value={editForm.lat} onChange={e => setEditForm({...editForm, lat: e.target.value})} required />
              <Input label="Longitude" type="number" step="any" value={editForm.lng} onChange={e => setEditForm({...editForm, lng: e.target.value})} required />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <Input label="Capacity (kWh)" type="number" value={editForm.capacityKwh} onChange={e => setEditForm({...editForm, capacityKwh: e.target.value})} required />
              <Input label="Total Battery Slots" type="number" value={editForm.batterySlots} onChange={e => setEditForm({...editForm, batterySlots: e.target.value})} required />
            </div>

            {renderScheduleEditor(editForm, setEditForm)}

            <div className="pt-4 flex justify-end space-x-3 border-t">
              <Button type="button" variant="outline" onClick={() => setIsEditOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={saving}>{saving ? 'Updating...' : 'Update Station'}</Button>
            </div>
          </form>
        </Modal>
      )}

      {/* Manage Slots Modal */}
      <Modal isOpen={isSlotOpen} onClose={() => setIsSlotOpen(false)} title={`Update Slots: ${selectedStation?.stationName}`}>
        <form onSubmit={handleUpdateSlots} className="space-y-4">
          <Input 
            label="Total Battery Slots" 
            type="number" 
            min={0}
            value={slotForm.totalSlots} 
            onChange={e => setSlotForm({ totalSlots: e.target.value })} 
            required 
          />
          <p className="text-xs text-gray-500 mt-2">Note: Existing bookings will remain unaffected.</p>
          <div className="pt-4 flex justify-end space-x-3">
            <Button type="button" variant="outline" onClick={() => setIsSlotOpen(false)}>Cancel</Button>
            <Button type="submit" disabled={saving}>{saving ? 'Updating...' : 'Update Slots'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default StationList;
