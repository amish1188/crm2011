import React from 'react';
import '../styles/MainStyle.css';

const ErrorMessage = (props) => <p style={props.style} className='ErrorMessage'>{props.children}</p>;

export default ErrorMessage;