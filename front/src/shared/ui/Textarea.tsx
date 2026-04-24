import styled from "styled-components";

const Textarea = styled.textarea`
  width: 300px;
  min-height: 120px;

  padding: 10px 12px;

  border: 1px solid #000;
  border-radius: 8px;

  font: inherit;
  font-size: 14px;

  resize: vertical;
  outline: none;

  transition: border-color 0.2s, box-shadow 0.2s;
    
    background-color: #fff;

  &:focus {
    border-color: #775D68;
    box-shadow: 0 0 0 3px rgba(119, 93, 104,0.25);
  }

  &::placeholder {
    color: #fff;
  }
`;

export default Textarea;