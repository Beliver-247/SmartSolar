import React, { useEffect, useState } from 'react';
import { usersApi } from '../../api/usersApi';
import Button from '../../components/ui/Button';
import Input from '../../components/ui/Input';
import Modal from '../../components/ui/Modal';
import LoadingSpinner from '../../components/ui/LoadingSpinner';
import ErrorBanner from '../../components/ui/ErrorBanner';
import EmptyState from '../../components/ui/EmptyState';
import toast from 'react-hot-toast';
import { Plus, Edit2, Trash2 } from 'lucide-react';

const UserList = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  // New User Form State
  const [newUser, setNewUser] = useState({
    username: '',
    password: '',
    fullName: '',
    role: 'Backoffice'
  });
  const [creating, setCreating] = useState(false);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [editUser, setEditUser] = useState(null);
  const [updating, setUpdating] = useState(false);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await usersApi.getAll();
      setUsers(data);
    } catch (err) {
      setError('Failed to load users.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setCreating(true);
    try {
      await usersApi.create(newUser);
      toast.success('User created successfully');
      setIsModalOpen(false);
      setNewUser({ username: '', password: '', fullName: '', role: 'Backoffice' });
      fetchUsers();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to create user');
    } finally {
      setCreating(false);
    }
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    setUpdating(true);
    try {
      await usersApi.update(editUser.id, {
        fullName: editUser.fullName,
        role: editUser.role,
        isActive: editUser.isActive
      });
      toast.success('User updated successfully');
      setIsEditOpen(false);
      fetchUsers();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update user');
    } finally {
      setUpdating(false);
    }
  };

  const handleDelete = async (user) => {
    if (!window.confirm(`Are you sure you want to delete user ${user.username}?`)) return;
    try {
      await usersApi.delete(user.id);
      toast.success('User deleted successfully');
      fetchUsers();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to delete user');
    }
  };

  const openEdit = (user) => {
    setEditUser(user);
    setIsEditOpen(true);
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold text-gray-800">Staff Management</h2>
        <Button onClick={() => setIsModalOpen(true)} className="flex items-center space-x-2">
          <Plus size={18} />
          <span>Add User</span>
        </Button>
      </div>

      <ErrorBanner message={error} />

      {loading ? (
        <div className="py-12"><LoadingSpinner /></div>
      ) : users.length === 0 ? (
        <EmptyState title="No Staff Found" description="There are no staff users registered in the system." />
      ) : (
        <div className="bg-white shadow overflow-hidden sm:rounded-lg border border-gray-200">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Username</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Full Name</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {users.map((user) => (
                <tr key={user.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{user.username}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{user.fullName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${user.role === 'Backoffice' ? 'bg-purple-100 text-purple-800' : 'bg-blue-100 text-blue-800'}`}>
                      {user.role}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {user.isActive ? (
                      <span className="text-green-600 font-medium">Active</span>
                    ) : (
                      <span className="text-red-600 font-medium">Inactive</span>
                    )}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
                    <Button variant="outline" className="text-xs py-1 px-2" onClick={() => openEdit(user)}>
                      <Edit2 size={14} className="inline mr-1"/> Edit
                    </Button>
                    <Button variant="danger" className="text-xs py-1 px-2" onClick={() => handleDelete(user)}>
                      <Trash2 size={14} className="inline mr-1"/> Delete
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New User">
        <form onSubmit={handleCreate} className="space-y-4">
          <Input 
            label="Username" 
            value={newUser.username} 
            onChange={e => setNewUser({...newUser, username: e.target.value})} 
            required 
          />
          <Input 
            label="Password" 
            type="password"
            value={newUser.password} 
            onChange={e => setNewUser({...newUser, password: e.target.value})} 
            required 
          />
          <Input 
            label="Full Name" 
            value={newUser.fullName} 
            onChange={e => setNewUser({...newUser, fullName: e.target.value})} 
            required 
          />
          <div className="flex flex-col">
            <label className="mb-1 text-sm font-medium text-gray-700">Role</label>
            <select 
              className="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-primary-500 focus:border-primary-500 sm:text-sm bg-white"
              value={newUser.role}
              onChange={e => setNewUser({...newUser, role: e.target.value})}
            >
              <option value="Backoffice">Backoffice</option>
              <option value="GridOperator">Grid Operator</option>
            </select>
          </div>
          <div className="pt-4 flex justify-end space-x-3">
            <Button type="button" variant="outline" onClick={() => setIsModalOpen(false)}>Cancel</Button>
            <Button type="submit" disabled={creating}>{creating ? 'Creating...' : 'Create User'}</Button>
          </div>
        </form>
      </Modal>

      {editUser && (
        <Modal isOpen={isEditOpen} onClose={() => setIsEditOpen(false)} title={`Edit User: ${editUser.username}`}>
          <form onSubmit={handleUpdate} className="space-y-4">
            <Input 
              label="Full Name" 
              value={editUser.fullName} 
              onChange={e => setEditUser({...editUser, fullName: e.target.value})} 
              required 
            />
            <div className="flex flex-col">
              <label className="mb-1 text-sm font-medium text-gray-700">Role</label>
              <select 
                className="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-primary-500 focus:border-primary-500 sm:text-sm bg-white"
                value={editUser.role}
                onChange={e => setEditUser({...editUser, role: e.target.value})}
              >
                <option value="Backoffice">Backoffice</option>
                <option value="GridOperator">Grid Operator</option>
              </select>
            </div>
            <div className="flex flex-col">
              <label className="mb-1 text-sm font-medium text-gray-700">Status</label>
              <select 
                className="px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-1 focus:ring-primary-500 focus:border-primary-500 sm:text-sm bg-white"
                value={editUser.isActive}
                onChange={e => setEditUser({...editUser, isActive: e.target.value === 'true'})}
              >
                <option value={true}>Active</option>
                <option value={false}>Inactive</option>
              </select>
            </div>
            <div className="pt-4 flex justify-end space-x-3">
              <Button type="button" variant="outline" onClick={() => setIsEditOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={updating}>{updating ? 'Updating...' : 'Update User'}</Button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
};

export default UserList;
