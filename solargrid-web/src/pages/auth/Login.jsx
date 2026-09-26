import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../auth/AuthContext';
import { authApi } from '../../api/authApi';
import Input from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import toast from 'react-hot-toast';

const Login = () => {
  const [credentials, setCredentials] = useState({ usernameOrNic: '', password: '' });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setCredentials(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await authApi.login(credentials);
      login(response.token, response.role, response.usernameOrNic);
      toast.success('Logged in successfully');
      navigate('/stations', { replace: true });
    } catch (error) {
      if (error.response && (error.response.status === 401 || error.response.status === 403)) {
        toast.error('Invalid credentials. Please try again.');
      } else {
        toast.error('An error occurred during login.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md text-center">
        <img src="/logo.png" alt="Smart Solar Microgrid" className="w-16 h-16 mx-auto mb-2 object-contain" />
        <h2 className="text-3xl font-extrabold text-gray-900">
          Smart Solar Microgrid
        </h2>
        <p className="mt-2 text-sm text-gray-600">
          Sign in to your account
        </p>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10 border border-gray-200">
          <form className="space-y-6" onSubmit={handleSubmit}>
            <Input
              label="Username or NIC"
              name="usernameOrNic"
              value={credentials.usernameOrNic}
              onChange={handleChange}
              required
            />
            <Input
              label="Password"
              type="password"
              name="password"
              value={credentials.password}
              onChange={handleChange}
              required
            />
            <div>
              <Button type="submit" className="w-full justify-center" disabled={loading}>
                {loading ? 'Signing in...' : 'Sign in'}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Login;
