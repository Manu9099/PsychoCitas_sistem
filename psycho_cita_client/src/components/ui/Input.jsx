import React from 'react'

const Input = React.forwardRef(
  (
    {
      label,
      hint,
      error,
      className = '',
      inputClassName = '',
      id,
      ...props
    },
    ref
  ) => {
    return (
      <div className={['w-full space-y-2', className].join(' ')}>
        {label ? (
          <label
            htmlFor={id}
            className="block text-sm font-medium text-slate-700"
          >
            {label}
          </label>
        ) : null}

        <input
          ref={ref}
          id={id}
          className={[
            'w-full rounded-2xl border bg-white px-4 py-3 text-sm text-slate-900 outline-none transition',
            error
              ? 'border-rose-400 focus:border-rose-500'
              : 'border-slate-300 focus:border-violet-500',
            'placeholder:text-slate-400',
            inputClassName,
          ].join(' ')}
          {...props}
        />

        {error ? (
          <p className="text-sm text-rose-600">{error}</p>
        ) : hint ? (
          <p className="text-sm text-slate-500">{hint}</p>
        ) : null}
      </div>
    )
  }
)

Input.displayName = 'Input'

export default Input